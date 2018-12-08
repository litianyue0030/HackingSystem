using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{
    public abstract class MainWeapon : Weapon
    { 
        public MainWeapon()
        {
            weaponType = WeaponType.MainWeapon;
        }
        
    }
}