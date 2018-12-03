using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{
    public abstract class BackWeapon : Weapon
    {
        public BackWeapon()
        {
            weaponType = WeaponType.BackWeapon;
        }

        public override void Refresh()
        {
            base.Refresh();/*
            if (Owner.ActiveWeapon == WeaponType.BackWeapon && SkillSystem.SkillCasting == null)
            {
                Owner.ActiveWeapon = WeaponType.MainWeapon;
            }
            else if (SkillSystem.SkillCasting != null && Owner.ActiveWeapon != WeaponType.BackWeapon)
            {
                Owner.ActiveWeapon = WeaponType.SubWeapon;
            }*/
        }
    }
}