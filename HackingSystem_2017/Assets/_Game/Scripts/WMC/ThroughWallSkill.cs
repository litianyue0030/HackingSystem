using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace WMC
{
    public class ThroughWallSkill : Skill
    {
        public override Animator anim { get; set; }

        public Transform playerTransform;

        [SerializeField]
        private float canPenetrateDistance = 10f;
        [SerializeField]
        private LayerMask layerMask;
        float radius = .5f;

        public ThroughWallSkill()
        {

            layerMask = LayerMask.GetMask("Wall");
            RuleCast = new RuleTrue();
            RuleComplete = new RuleTrue();
            RuleEnd = new RuleTrue();
        }

        public override void AfterComplete()
        {

        }

        public override void Casting()
        {

        }

        public override void Complete()
        {

        }

        public override void Decast()
        {

        }

        public override void Enter()
        {
            //anim.GetNextAnimatorClipInfo(0);
            playerTransform = owner.owner.Owner.transform;
            DisplacementWall(playerTransform, playerTransform.forward, layerMask, radius);
        }

        public override void Exit()
        {

        }

        public override void InActive()
        {

        }

        void DisplacementWall(Transform transform, Vector3 direction, LayerMask layerMask, float r)
        {
            direction.y = 0;
            int count = 0;
            RaycastHit hit1;
            if (Physics.Raycast(transform.position, direction, out hit1, r + direction.magnitude * Time.fixedDeltaTime, layerMask))
            {
                RaycastHit hit2;
                Vector3 pos = transform.position + direction.normalized * canPenetrateDistance;
                Debug.Log(pos);
                while (Vector3.Dot(pos - transform.position, direction) > 0)
                {
                    count++;
                    bool IsHit = Physics.Raycast(pos, -direction, out hit2, canPenetrateDistance, layerMask);
                    if (IsHit && hit1.transform == hit2.transform)
                    {
                        Debug.Log(hit2.transform);
                        transform.position = hit2.point + direction * (Time.fixedDeltaTime + r);
                        break;
                    }
                    else
                    {
                        pos -= direction * Time.fixedDeltaTime;
                    }
                }
            }
        }

    }
}