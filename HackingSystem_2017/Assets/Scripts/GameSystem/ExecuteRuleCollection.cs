using UnityEngine;
using UnityEditor;
namespace HackingSystem
{
    public class RuleSkillInActive : ExecuteRule
    {
        Bot owner;
        public RuleSkillInActive(Bot owner)
        {
            this.owner = owner;
        }
        public override bool RuleExecute()
        {
            return owner.WeaponSystem.skillCasting == null;
        }
        
    }

    /// <summary>
    /// 时间条件，经过一定时间之后满足条件
    /// </summary>
    public class RuleTimeOver:ExecuteRule
    {
        float StartTime;//起始时间
        float timePause;//游戏暂停时间
        public float TimeElapse { get; set; }//满足条件必须经过的时间
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TimeElapse">满足条件必须经过的时间</param>
        public RuleTimeOver(float TimeElapse)
        {
            this.TimeElapse = TimeElapse;
        }

        public override void Reset()
        {
            StartTime = Time.time;
            timePause = GameSystem.TimePausedTotal;
        }

        public override bool RuleExecute()
        {
            return Time.time - StartTime - (GameSystem.TimePausedTotal - timePause) > TimeElapse;
        }
    }

    public class RuleFalse : ExecuteRule
    {
        public override bool RuleExecute()
        {
            return false;
        }
    }

    public class RuleTrue : ExecuteRule
    {
        public override bool RuleExecute()
        {
            return true;
        }
    }
}