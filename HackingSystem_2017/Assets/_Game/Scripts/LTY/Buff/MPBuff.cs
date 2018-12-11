using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;
using HackingSystem.Tools;

namespace LTY
{
    /// <summary>
    /// 能量MP每秒增加/减少buff
    /// </summary>
    public class MPBuff : Buff
    {
        private int m_mpDelta;
        private Timer m_timer;
        public MPBuff(Bot source, float time, BuffType buffType, BuffSystem owner, int mpDelta)
        {
            m_mpDelta = mpDelta;
            this.Source = source;
            this.Owner = owner;
            this.BuffType = buffType;
            m_timer = new Timer();
            BuffEndRule = new RuleTimeOver(time);
        }

        public override void Enter()
        {
            Owner.owner.Interrupt++;
            m_timer.SetExpiredTime(Time.time + 1);
        }

        public override void Execute()
        {
            if (m_timer.IsExpired(Time.time))
            {
                Owner.owner.Abilities.MP.Value += m_mpDelta;
                m_timer.SetExpiredTime(Time.time + 1);
            }
        }

        public override void Exit()
        {
            Owner.owner.Interrupt--;
        }
    }
}
