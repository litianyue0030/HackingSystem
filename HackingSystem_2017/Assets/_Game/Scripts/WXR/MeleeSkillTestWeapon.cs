using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace HackingSystem.Melee
{
    public class MeleeDefendSkillTestWeapon : BallWeapon
    {
        public Animator a;

        static MeleeDefendSkillTestWeapon()
        {
        }

        public MeleeDefendSkillTestWeapon() : base()
        {
            List<Skill> skills = new List<Skill>();
            skills.Add(new MeleeDefendSkill());

            foreach (var item in skills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = skills;
        }

        /// <summary>
        /// 该函数会在该武器的Owner发生改变的时候调用，非构造时调用
        /// </summary>
        public override void initialization()
        {
            if (((Eriya.Eriya)Owner).eriyaMode == Eriya.EriyaMode.Hacking)
            {
                //动画初始化
                //((Eriya.Eriya)Owner).eriyaMode = Eriya.EriyaMode.Bot;
            }
            a = Owner.Animator;
            foreach (var item in SkillSystem.skills)
            {
                item.anim = a;
            }
        }

        public override void Refresh()
        {
            if (((HackingSystem.Eriya.Eriya)Owner).eriyaMode == HackingSystem.Eriya.EriyaMode.Bot)
            {
                Debug.Log("Bot");
                return;
            }
            base.Refresh();

            // 盾技能RuleCast中有按键需求（长按而非单击按键）
            // 故此处无Control按键条件，直接在SkillSystem中Cast
            SkillSystem.Cast(0);
            if (Control.MainWPAttackLDown())
            {
                //SkillSystem.Cast(0);
            }
            if (Control.MainWPAttackRDown())
            {
                //SkillSystem.Cast(1);
            }
            if (Control.BackArrowDown())
            {
                //SkillSystem.Cast(2);
            }
            if (Control.CoreArrowDown())
            {
                //SkillSystem.Cast(3);
            }
            if (Control.ShiftHackDown())
            {
                //SkillSystem.Cast(4);
            }
        }

        public override void dispose()
        {
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class MeleeDumyMainWeapon : MainWeapon
    {
        public MeleeDumyMainWeapon() : base()
        {
            List<Skill> skills = new List<Skill>();

            foreach (var item in skills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = skills;
        }

        public override void dispose()
        {
            
        }

        public override void initialization()
        {
            
        }
    }

    public class MeleeDumySubWeapon : SubWeapon
    {
        public MeleeDumySubWeapon() : base()
        {
            List<Skill> skills = new List<Skill>();

            foreach (var item in skills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = skills;
        }

        public override void dispose()
        {

        }

        public override void initialization()
        {

        }
    }

    public class MeleeDumyBackWeapon : BackWeapon
    {
        public MeleeDumyBackWeapon() : base()
        {
            List<Skill> skills = new List<Skill>();

            foreach (var item in skills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = skills;
        }

        public override void dispose()
        {

        }

        public override void initialization()
        {

        }
    }
}
