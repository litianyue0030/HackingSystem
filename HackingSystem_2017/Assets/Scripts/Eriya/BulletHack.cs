using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem.Eriya
{
    public class BulletHack : MonoBehaviour
    {
        internal SkillBallHack owner;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "EnemyAndResources")
            {
                //寻找最近的敌人连线
                if (owner.HackEnemy == null)
                {
                    owner.HackEnemy = other.GetComponent<Bot>();
                    return;
                }
                Vector3 enemyp = owner.HackEnemy.transform.position;
                Vector3 ownerp = owner.owner.owner.Owner.transform.position;
                Vector3 Curp = other.transform.position;
                if ((enemyp - ownerp).magnitude > (Curp - ownerp).magnitude)
                {
                    owner.HackEnemy = other.GetComponent<Bot>();
                }
            }
        }
    }
}