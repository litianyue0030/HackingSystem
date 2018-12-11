using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace LTY
{
    /// <summary>
    /// 一段时间内增加/降低移速Buff
    /// </summary>
    public class MoveSpeedBuff : Buff
    {
        float m_moveSpeedRate;
        float m_originalMoveSpeed;
        public MoveSpeedBuff(Bot source, float time, BuffSystem owner, float moveSpeedRate, BuffType buffType)
        {
            m_moveSpeedRate = moveSpeedRate;
            this.Source = source;
            this.Owner = owner;
            this.BuffType = buffType;
            this.BuffEndRule = new RuleTimeOver(time);
            m_originalMoveSpeed = Owner.owner.Abilities.MoveSpeedRate;
        }

        public override void Enter()
        {
            Owner.owner.Interrupt++;
            Owner.owner.Abilities.MoveSpeedRate = m_moveSpeedRate;
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            Owner.owner.Abilities.MoveSpeedRate = m_originalMoveSpeed;
            Owner.owner.Interrupt--;
        }
    }
}
