
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HackingSystem
{
    public abstract class Skill : ScriptableObject
    {
        public virtual Animator anim { get; set; }

        //owner
        public SkillSystem owner { get; set; }
        //HitDefend

        //Enter,Execute,Complete,AfterComplete,Exit
        public abstract void Enter();
        public abstract void Casting();
        public abstract void Exit();
        public abstract void Complete();
        public abstract void AfterComplete();
        //Decast，Inactive
        public abstract void Decast();
        public abstract void InActive();

        /// <summary>
        /// 技能发动的条件，在技能发动成功的时候重置
        /// </summary>
        public ExecuteRule RuleCast;
        /// <summary>
        /// 技能完成的条件，在技能释放的时候进行重置
        /// </summary>
        public ExecuteRule RuleComplete;
        /// <summary>
        /// 技能结束的条件，在技能完成的时候重置
        /// </summary>
        public ExecuteRule RuleEnd;


        //
        public SkillActionPhase CurrentPhase;
    }

    public enum SkillActionPhase
    {
        InActive,
        Casting,
        AfterComplete
    }
}
