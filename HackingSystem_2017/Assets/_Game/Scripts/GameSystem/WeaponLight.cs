using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{
    public class WeaponLight : MonoBehaviour
    {
        public Weapon weapon;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Bot b = other.GetComponent<Bot>();
                switch (weapon.weaponType)
                {
                    case WeaponType.MainWeapon:
                        b.WeaponSystem.MainWeapon = (MainWeapon)weapon;
                        break;
                    case WeaponType.SubWeapon:
                        b.WeaponSystem.SubWeapon = (SubWeapon)weapon;
                        break;
                    case WeaponType.CoreWeapon:
                        b.WeaponSystem.CoreWeapon = (CoreWeapon)weapon;
                        break;
                    case WeaponType.BackWeapon:
                        b.WeaponSystem.BackWeapon = (BackWeapon)weapon;
                        break;
                    default:
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}