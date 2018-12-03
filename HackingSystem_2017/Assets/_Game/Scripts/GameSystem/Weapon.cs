using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem {
    public abstract class Weapon{

        public RuntimeAnimatorController CurAnimController;
        public bool AnimActive;

        Bot owner;
        public WeaponType weaponType { get; protected set; }
        public Bot Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                initialization();
                //激活被动
            }
        }

        public Weapon()
        {
            SkillSystem = new SkillSystem();
        }

        public Item itemOwner
        {
            get;set;
        }
        
        //Durability耐久度

        //Skills enter-execute-complete-AfterComplete-exit 硬直抗性
        SkillSystem skillSystem;
        public SkillSystem SkillSystem
        {
            get
            {
                return skillSystem;
            }
            set
            {
                skillSystem = value;
                skillSystem.owner = this;
            }
        }

        /// <summary>
        /// 解除武器时的操作
        /// </summary>
        public abstract void dispose();

        /// <summary>
        /// 武器装备上的时候发生，初始化操作
        /// </summary>
        public abstract void initialization();
        
        public virtual void Refresh()
        {
            //刷新技能
            SkillSystem.Refresh();
        }
    }
}