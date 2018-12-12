using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace WMC
{
    /// <summary>
    /// 闪现
    /// </summary>
    public class FlashSkill : Skill
    {
        public override Animator anim { get; set; }

        public Transform playerTransform;

        [SerializeField]
        private float canPenetrateDistance = 2f;
        [SerializeField]
        private LayerMask layerMask;
        float radius = 0.5f;

        public FlashSkill()
        {
           
            layerMask = 10;
            RuleCast = new RuleTrue();
            RuleComplete = new RuleTrue();
            RuleEnd = new RuleTrue();
            //RuleComplete = new FlashSkillRuleComplete();
            //RuleEnd = new FlashSkillRuleEnd();
        }

        public override void AfterComplete()
        {

        }

        public override void Casting()
        {

        }

        public override void Complete()
        {
            //进入冷却
        }

        public override void Decast()
        {

        }

        public override void Enter()
        {
            playerTransform = owner.owner.Owner.transform;
            //anim.GetNextAnimatorClipInfo(0);
            DisplacementWall(playerTransform,playerTransform.forward, layerMask);
        }

        public override void Exit()
        {

        }

        public override void InActive()
        {

        }

        /// <summary>
        /// 闪现技能，射线检测前方是否有墙体，如果没有直接闪过去，如果有就撞墙，在墙的前方一段距离现身
        /// </summary>
        /// <param name="transform">玩家的transform</param>
        /// <param name="direction">玩家的前方</param>
        /// <param name="layerMask">墙体的层</param>
        void DisplacementWall(Transform transform, Vector3 direction, LayerMask layerMask)
        {
            direction.y = 0;
            RaycastHit hit1;
            if (Physics.Raycast(transform.position, direction, out hit1, canPenetrateDistance, layerMask))
            {
                Debug.Log(hit1.point);
                Vector3 pRadiusPos = (hit1.point - transform.position).normalized;
                transform.position = hit1.point - pRadiusPos;
            }
            else
                transform.position += direction.normalized * canPenetrateDistance;
        }
    }
}