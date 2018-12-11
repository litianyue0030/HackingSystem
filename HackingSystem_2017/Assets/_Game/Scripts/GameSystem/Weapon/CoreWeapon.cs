using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackingSystem
{
    public abstract class CoreWeapon : Weapon
    {
        public CoreWeapon()
        {
            weaponType = WeaponType.CoreWeapon;
        }
        public override void Refresh()
        {
            base.Refresh();/*
            if (Owner.ActiveWeapon == WeaponType.CoreWeapon && SkillSystem.SkillCasting == null)
            {
                Owner.ActiveWeapon = WeaponType.MainWeapon;
            }
            else if (SkillSystem.SkillCasting != null && Owner.ActiveWeapon != WeaponType.CoreWeapon)
            {
                Owner.ActiveWeapon = WeaponType.CoreWeapon;
            }*/
        }
    }
}