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
        public enum DistanceJudgeType
        {
            TypeMouse_WallEffect,
            TypeMouse_GroundEffect,
            TypeNormalDistance3D,
            TypeNormalDistance2D
        }

        public DistanceJudgeType judgeType;
        public GameObject SkillJudgeBullet;
        public GameObject CastingEffect;
        public GameObject AfterCompleteEffect;
        public string AnimBeforeCompleteName;
        public string AnimAfterCompleteName;
        bool Initialized = false;

        GameObject curCastE;
        GameObject curAfterComE;

        private void OnEnable()
        {
            switch (judgeType)
            {
                case DistanceJudgeType.TypeMouse_WallEffect:
                case DistanceJudgeType.TypeMouse_GroundEffect:

                case DistanceJudgeType.TypeNormalDistance3D:
                    calc = calc_TypeNormalDistance;
                    break;
                case DistanceJudgeType.TypeNormalDistance2D:
                    calc = calc_TypeNormalDistance_Ground;
                    break;
                default:
                    break;
            }
        }

        Vector3 calc_TypeNormalDistance_Ground()
        {
            return (Vector3)owner.owner.Owner.Diration2 * Distance + owner.owner.Owner.transform.position;
        }

        Vector3 calc_TypeNormalDistance()
        {
            return (Vector3)owner.owner.Owner.Direction * Distance + owner.owner.Owner.transform.position;
        }

        public override void AfterComplete()
        {
        }

        public override void Casting()
        {
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
        }

        public override void Decast()
        {
        }

        public override void Enter()
        {
            if (!Initialized)
            {
                Initialized = true;
                
            }
            owner.owner.Owner.Animator.PlayInFixedTime(AnimBeforeCompleteName);
            if (CastingEffect)
            {
                curCastE = Instantiate(CastingEffect, owner.owner.Owner.transform);
            }
        }

        public override void Exit()
        {
            if (curCastE)
            {
                Destroy(curCastE);
                curCastE = null;
            }
            if (curAfterComE)
            {
                Destroy(curAfterComE);
                curCastE = null;
            }
        }

        public override void InActive()
        {
        }
    }
}