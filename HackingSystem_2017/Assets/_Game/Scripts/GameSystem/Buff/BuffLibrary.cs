using UnityEngine;
using UnityEditor;

namespace HackingSystem
{
    /// <summary>
    /// 硬直带来的击退,不是击飞
    /// </summary>
    public class BuffKnockOver : Buff
    {
        Vector3 Velocity;
        Vector3 a;
        Rigidbody rd;
        public BuffKnockOver(Bot source,float time,Vector3 Velocity)
        {
            BuffEndRule = new RuleTimeOver(time);
            this.Velocity = Velocity;
            a = Velocity / time;
            a.y = 0;
            this.BuffType = BuffType.debuff;
        }

        public override void Enter()
        {
            Owner.owner.Interrupt++;
            rd = Owner.owner.GetComponent<Rigidbody>();
        }

        public override void Execute()
        {
            Velocity -= a * Time.deltaTime;
            Velocity.y = rd.velocity.y;
            rd.velocity = Velocity;
        }

        public override void Exit()
        {
            Owner.owner.Interrupt--;
        }
    }

    /// <summary>
    /// 击飞，击飞碰到地面会陷入倒地1.2s
    /// </summary>
    public class BuffKnockOut : Buff
    {
        RuleTouchGround tg;
       
        Vector3 ve;
        public BuffKnockOut(Bot source,Vector3 Velocity)
        {
            this.Source = source;
            ve = Velocity;
            BuffEndRule = new RuleFalse();
            this.BuffType = BuffType.debuff;
        }

        public override void Enter()
        {
            Owner.owner.Interrupt++;
            BuffEndRule = new RuleTouchGround(Owner.owner);
            Owner.owner.GetComponent<Rigidbody>().velocity = ve;
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
            if (tg)
            {
                Owner.AddBuff(new BuffDownGround(Source));
            }

            Owner.owner.Interrupt--;
        }
    }

    /// <summary>
    /// 倒地,倒地过程中无视大部分伤害
    /// </summary>
    class BuffDownGround : Buff
    {

        public BuffDownGround(Bot source)
        {
            this.Source = source;
            this.BuffType = BuffType.debuff;
            BuffEndRule = new RuleTimeOver(1.2f);
        }

        public override void Enter()
        {
            Owner.owner.Interrupt++;
            Owner.owner.BattleSystem.OnBeingDamageByItem += Owner_OnBeingDamageByItem;
            Owner.owner.BattleSystem.OnBeingDamageBySkill += Owner_OnBeingDamageBySkill;
        }

        private void Owner_OnBeingDamageBySkill(object sender, DamageEventArgs<Skill> e)
        {
            if (!e.DownDamage)
            {
                e.Cancel = true;
            }
        }

        private void Owner_OnBeingDamageByItem(object sender, DamageEventArgs<Item> e)
        {
            if (!e.DownDamage)
            {
                e.Cancel = true;
            }
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
            Owner.owner.Interrupt--;
            Owner.owner.BattleSystem.OnBeingDamageByItem -= Owner_OnBeingDamageByItem;
            Owner.owner.BattleSystem.OnBeingDamageBySkill -= Owner_OnBeingDamageBySkill;
        }
    }

    public class RuleTouchGround:ExecuteRule
    {
        Bot b;
        public RuleTouchGround(Bot owner)
        {
            b = owner;
        }
        public override bool RuleExecute()
        {
            return !b.OnAir;
        }
    }
}