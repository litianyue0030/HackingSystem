using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace LTY
{
    /// <summary>
    /// 一段时间内免伤
    /// </summary>
    public class FreeFromInjuryBuff : Buff
    {
        public FreeFromInjuryBuff(Bot source, float time, BuffSystem owner)
        {
            this.Source = source;
            this.Owner = owner;
            this.BuffType = BuffType.buff;
            BuffEndRule = new RuleTimeOver(time);
        }


        public override void Enter()
        {
            Owner.owner.Interrupt++;
            Owner.owner.Abilities.HP.OnValueChange += HP_OnValueChange;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            Owner.owner.Abilities.HP.OnValueChange -= HP_OnValueChange;
            Owner.owner.Interrupt--;
        }

        private void HP_OnValueChange(object sender, ValueChingingEventArgs<int> e)
        {
            e.CancelChange = true;
        }
    }
}
