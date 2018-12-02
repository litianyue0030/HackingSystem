using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{
    public abstract class SubWeapon : Weapon
    {
        public SubWeapon()
        {
            weaponType = WeaponType.SubWeapon;
        }

        public override void Refresh()
        {
            base.Refresh();/*
            if (Owner.ActiveWeapon == WeaponType.SubWeapon && SkillSystem.SkillCasting == null)
            {
                Owner.ActiveWeapon = WeaponType.MainWeapon;
            }
            else if (SkillSystem.SkillCasting != null && Owner.ActiveWeapon != WeaponType.SubWeapon)
            {
                Owner.ActiveWeapon = WeaponType.SubWeapon;
            }*/
        }
    }
}