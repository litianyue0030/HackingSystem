using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackingSystem
{
    public class SkillSystem
    {
        const string AnimSkillNone="Stand";
        //技能列表
        public List<Skill> skills;
        
        public Skill SkillCasting
        {
            get;
            private set;
        }

        public Weapon owner
        {
            get; set;
        }

        
        public void Cast(int SkillID)
        {
            if (skills[SkillID].RuleCast)
            {
                owner.Owner.WeaponSystem.DecastSkill();
                skills[SkillID].RuleComplete.Reset();
                skills[SkillID].RuleCast.Reset();
                skills[SkillID].Enter();
                SkillCasting = skills[SkillID];
                skills[SkillID].CurrentPhase = SkillActionPhase.Casting;
            }
        }

        public void SkillDecast()
        {
            if (SkillCasting == null)
            {
                return;
            }
            SkillCasting.Decast();
            SkillCasting.Exit();
            SkillCasting.CurrentPhase = SkillActionPhase.InActive;
            SkillCasting = null;
        }

        //刷新技能列表
        public void Refresh()
        {

            //遍历技能
            //执行InActive（）如果技能是激活，不执行InActive
            //当前使用哪个技能，调用Execute-AfterComplete 判断下是否满足技能结束/咏唱成功条件
            foreach (Skill ski in skills)
            {
                if (ski.CurrentPhase == SkillActionPhase.InActive)
                {
                    ski.InActive();
                }
            }
            if (SkillCasting == null)
            {
                return;
            }
            switch (SkillCasting.CurrentPhase)
            {
                case SkillActionPhase.Casting:

                    if (SkillCasting.RuleComplete)
                    {
                        SkillCasting.Complete();
                        SkillCasting.RuleEnd.Reset();
                        SkillCasting.CurrentPhase = SkillActionPhase.AfterComplete;
                    }
                    else
                    {
                        SkillCasting.Casting();
                    }
                    break;
                case SkillActionPhase.AfterComplete:

                    if (SkillCasting.RuleEnd)
                    {
                        SkillCasting.Exit();
                        SkillCasting.CurrentPhase = SkillActionPhase.InActive;
                        SkillCasting = null;
                    }
                    else
                    {
                        SkillCasting.AfterComplete();
                    }
                    break;
                default:
                    break;
            }
            //
        }
        
    }
}
