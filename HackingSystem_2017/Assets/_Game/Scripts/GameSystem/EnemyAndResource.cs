using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{

    public abstract class EnemyAndResource : Bot
    {
        public GameObject WeaponLight;

        public override void Initialzation()
        {
            base.Initialzation();
            tag = "EnemyAndResources";
            weaponList = new List<Weapon>();
        }

        public List<Weapon> weaponList;

        /// <summary>
        /// 黑入时的武器光柱
        /// </summary>
        GameObject CurLight = null;
        /// <summary>
        /// 被黑入的函数，生成武器光柱后销毁目标
        /// </summary>
        public virtual void BeHack()
        {
            GameObject wl = Instantiate(WeaponLight, transform.position, Quaternion.identity);
            wl.GetComponent<WeaponLight>().weapon = weaponList[(int)(Random.value * weaponList.Count)];
            Destroy(gameObject);
        }
    }
}