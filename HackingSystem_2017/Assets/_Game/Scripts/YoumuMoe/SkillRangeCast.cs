using UnityEngine;
using UnityEditor;
using System.IO;

namespace HackingSystem
{
    [CreateAssetMenu(fileName = "new SkillRangeCast", menuName = "Skills/SkillRangeCast", order = 0)]
    public class SkillRangeCast : Skill
    {
        delegate Vector3 JudgeCalc();

        JudgeCalc calc;

        public float Distance;
        /// <summary>
        /// 判定标准
        /// </summary>
        public enum DistanceJudgeType
        {
            /// <summary>
            /// 墙面判定允许，按Direction面向位置判定
            /// </summary>
            TypedDirection_WallEffect,
            /// <summary>
            /// 只允许地面判定，按Direction面向位置判定
            /// </summary>
            TypeDirection_GroundEffect,
            /// <summary>
            /// 固定距离，判定
            /// </summary>
            TypeNormalDistance3D,
            /// <summary>
            /// 固定距离判定（2D）
            /// </summary>
            TypeNormalDistance2D
        }

        /// <summary>
        /// 判定的鼠标光标
        /// </summary>
        public GameObject JudgeIcon;
        public GameObject FailJudgeIcon;

        GameObject CurJudgeIcon = null;
        GameObject CurFailJudgeIcon = null;

        /// <summary>
        /// 判定技能释放是否有效，该属性仅会适用于Distance Judge Type为Type Direction模式
        /// </summary>
        public bool JudgeSuccess;
        bool JudgeChange = false;

        public DistanceJudgeType judgeType;
        /// <summary>
        /// 判定的子弹（含特效）
        /// </summary>
        public GameObject SkillJudgeBullet;
        /// <summary>
        /// 释放中的特效
        /// </summary>
        public GameObject CastingEffect;
        /// <summary>
        /// 后摇特效
        /// </summary>
        public GameObject AfterCompleteEffect;
        /// <summary>
        /// 释放中动画,没有则设置为“”
        /// </summary>
        public string AnimBeforeCompleteName = "";
        /// <summary>
        /// 后摇动画，没有则设置为“”
        /// </summary>
        public string AnimAfterCompleteName = "";
        bool Initialized = false;

        GameObject curCastE;
        GameObject curAfterComE;
        Bot botO;

        public override SkillSystem owner
        {
            get
            {
                return base.owner;
            }

            set
            {
                base.owner = value;
                botO = value.owner.Owner;
            }
        }

        private void OnEnable()
        {
            switch (judgeType)
            {
                case DistanceJudgeType.TypedDirection_WallEffect:
                case DistanceJudgeType.TypeDirection_GroundEffect:
                    calc = calc_TypeDirection_Wall;
                    //使用这种判定方式时完成技能必须按下左键
                    RuleComplete = RuleComplete & new RuleClickL();
                    CurJudgeIcon = Instantiate(JudgeIcon);
                    CurFailJudgeIcon = Instantiate(FailJudgeIcon);
                    break;
                case DistanceJudgeType.TypeNormalDistance3D:
                    calc = calc_TypeNormalDistance;
                    JudgeIcon = FailJudgeIcon = null;
                    break;
                case DistanceJudgeType.TypeNormalDistance2D:
                    calc = calc_TypeNormalDistance_Ground;
                    JudgeIcon = FailJudgeIcon = null;
                    break;
                default:
                    break;
            }
        }

        Vector3 calc_TypeNormalDistance_Ground()
        {
            return (Vector3)botO.Diration2 * Distance + botO.transform.position;
        }

        Vector3 calc_TypeNormalDistance()
        {
            return botO.Direction * Distance + botO.transform.position;
        }

        Vector3 calc_TypeDirection_Wall()
        {
            Vector3 vPo = botO.transform.Find("CameraGroup").position;
            var Hits = Physics.RaycastAll(vPo, botO.Direction, Distance);
            
            foreach (var item in Hits)
            {
                /*
                if (item.collider.tag == "Wall")
                {
                    if (!JudgeSuccess)
                    {
                        JudgeChange = true;
                    }
                    JudgeSuccess = true;
                    return item.point;
                }
                if(item.collider.tag == "Ground")
                {
                    if (!JudgeSuccess)
                    {
                        JudgeChange = true;
                    }
                    JudgeSuccess = true;
                    return item.point;
                }*/
                if (item.collider.tag != "Player")
                {
                    if (!JudgeSuccess)
                    {
                        JudgeChange = true;
                    }
                    JudgeSuccess = true;
                    return item.point;
                }
            }
            if (JudgeSuccess)
            {
                JudgeChange = true;
            }
            JudgeSuccess = false;
            var Hit = Physics.RaycastAll(vPo + botO.Direction * Distance, botO.Direction, Distance);
            foreach (var item in Hit)
            {
                if (item.collider.tag!="Player")
                {
                    return item.point;
                }
            }

            return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        public override void AfterComplete()
        {
        }

        public override void Casting()
        {
            curCastE.transform.eulerAngles = new Vector3(0, botO.AngleDirection.y);
            //鼠标指向的范围技能定位
            if (judgeType == DistanceJudgeType.TypeDirection_GroundEffect || judgeType == DistanceJudgeType.TypedDirection_WallEffect)
            {
                Vector3 ZB = calc();
                CurJudgeIcon.transform.position = ZB;
                CurFailJudgeIcon.transform.position = ZB;

                if (JudgeChange)
                {
                    JudgeChange = false;
                    CurJudgeIcon.SetActive(JudgeSuccess);
                    CurFailJudgeIcon.SetActive(!JudgeSuccess);
                }
            }
        }

        public override void Complete()
        {
            owner.owner.Owner.Animator.PlayInFixedTime(AnimAfterCompleteName);
            Vector3 v = calc();
            GameObject.Instantiate(SkillJudgeBullet, v, Quaternion.LookRotation(owner.owner.Owner.Diration2));
            if (AfterCompleteEffect)
            {
                curAfterComE = Instantiate(AfterCompleteEffect, owner.owner.Owner.transform);
            }


            
            if (curCastE)
            {
                curCastE.SetActive(false);
            }

            if (curAfterComE)
            {
                curCastE.SetActive(true);
                curCastE.transform.SetParent(owner.owner.Owner.transform);
            }
            else if (AfterCompleteEffect)
            {
                Instantiate(AfterCompleteEffect, owner.owner.Owner.transform);
            }

            if (!CurJudgeIcon)
            {
                return;
            }
            CurJudgeIcon.SetActive(true);
            CurFailJudgeIcon.SetActive(true);
        }

        public override void Decast()
        {
            if (curCastE)
            {
                curCastE.SetActive(false);
            }
            if (curAfterComE)
            {
                curAfterComE.SetActive(false);
            }
        }

        public override void Enter()
        {
            if (!Initialized)
            {
                Initialized = true;

            }
            owner.owner.Owner.Animator.PlayInFixedTime(AnimBeforeCompleteName);


            if (curCastE)
            {
                curCastE.SetActive(true);
                curCastE.transform.SetParent(owner.owner.Owner.transform);
            }
            else if (CastingEffect)
            {
                Instantiate(CastingEffect, owner.owner.Owner.transform);
            }

            if (!CurJudgeIcon)
            {
                return;
            }
            CurJudgeIcon.SetActive(true);
            CurFailJudgeIcon.SetActive(true);
        }

        public override void Exit()
        {
            if (curCastE)
            {
                curCastE.SetActive(false);
            }
            if (curAfterComE)
            {
                curAfterComE.SetActive(false);
            }
            if (JudgeIcon)
            {
                JudgeIcon.SetActive(false);
            }
            if (FailJudgeIcon)
            {
                FailJudgeIcon.SetActive(false);
            }
        }

        public override void InActive()
        {
        }
    }

    /// <summary>
    /// 按下主武器键且判定成功时发生
    /// </summary>
    class RuleClickL : ExecuteRule
    {
        SkillRangeCast RangeCast;
        public override bool RuleExecute()
        {
            return Control.MainWPAttackLDown() && RangeCast.JudgeSuccess;
        }
    }
}