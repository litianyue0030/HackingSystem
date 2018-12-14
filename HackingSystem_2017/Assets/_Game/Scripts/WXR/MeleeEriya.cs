using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem.Eriya;
using LTY;

namespace HackingSystem.Melee
{
    public class MeleeEriya : HackingSystem.Eriya.Eriya
    {
        public override void Initialzation()
        {
            Physics.gravity = new Vector3(0, -30, 0);
            base.Initialzation();
            tag = "Player";
            //Action = new PlayerController();
            Action = new PlayerController();

            abilities = new Abilities(this, 100, 200);
            abilities.KnockBackDefend = 150;

            WeaponSystem.BallWeapon = new MeleeDefendSkillTestWeapon();

            eriyaMode = EriyaMode.Hacking;
            WeaponSystem.MainWeapon = new Dustbin.MainWeaponDusbinTop();
            WeaponSystem.SubWeapon = new Dustbin.SubWeaponDusbinTop();
            WeaponSystem.BackWeapon = new Dustbin.BackWeaponDusbinTop();
        }
    }
}