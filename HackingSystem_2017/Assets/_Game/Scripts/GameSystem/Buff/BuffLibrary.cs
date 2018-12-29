using UnityEngine;
using UnityEditor;

namespace HackingSystem
{
    #region 僵直相关
    /// <summary>
    /// 硬直带来的击退,不是击飞
    /// </summary>
    public class BuffKnockOver : Buff
    {
        Vector3 Velocity;
        Vector3 a;
        Rigidbody rd;
        public BuffKnockOver(Bot source, float time, Vector3 Velocity)
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

    #endregion

    #region 控制相关（基类框架）
    /// <summary>
    /// 失去控制，并且附加新的控制行为的buff,重复添加时，新行为会把旧行为覆盖
    /// </summary>
    public class BuffOverControl:Buff
    {
        /// <summary>
        /// OverControl的行为,该行为拥有参数BuffOwner，该参数为行为的Buff持有者
        /// </summary>
        public GameAction<Bot> OverControlAction;

        public BuffOverControl(string BuffName,GameAction<Bot> OverControlAction)
        {
            this.BuffName = BuffName;
            this.OverControlAction = OverControlAction;
            OverControlAction.ActionParams.Add("BuffOwner", this);
        }

        public override void Enter()
        {
            Owner.owner.OverControlAdd(this);
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
            Owner.owner.OverControlRemove(this);
        }
    }

    /// <summary>
    /// 无法行动的Buff类型
    /// </summary>
    public class BuffStun : Buff
    {
        public override void Enter()
        {
            Owner.owner.Interrupt++;
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
            Owner.owner.Interrupt--;
        }
    }
    #endregion

    #region 恐惧
    /// <summary>
    /// 恐惧行为,随机往后面跑
    /// </summary>
    public class ActionHorror:GameAction<Bot>
    {
        Vector2 ADirection;
        public override void Enter()
        {
            BuffOverControl owner = (BuffOverControl)ActionParams["BuffOwner"];
            Vector3 Direction = Executor.transform.position - owner.Source.transform.position;
            Direction.y = 0;
            float Eular = (Random.value - 0.5f) * 60;
            ADirection = GameSystem.ConvertDirationTOEularAngles(Direction);
            ADirection.y += Eular;
            Executor.AngleDirection = ADirection;
        }
        public override void Execute()
        {
            Executor.MoveFrame(Vector3.forward);
        }
    }
    public class BuffHorror : BuffOverControl
    {
        public BuffHorror(string BuffName) : base(BuffName, new ActionHorror())
        {
        }
        public BuffHorror() : base("Horror", new ActionHorror())
        {
        }
    }
    #endregion

    #region 魅惑
    /// <summary>
    /// 魅惑行为,往敌人跑
    /// </summary>
    public class ActionCharm : GameAction<Bot>
    {
        public override void Enter()
        {
            
        }
        public override void Execute()
        {
            Executor.Direction = ((BuffOverControl)ActionParams["BuffOwner"]).Source.transform.position - Executor.transform.position; ;
            Executor.MoveFrame(Vector3.forward);
        }
    }
    public class BuffCharm : BuffOverControl
    {
        public BuffCharm(string BuffName) : base(BuffName, new ActionCharm())
        {
        }
        public BuffCharm() : base("Charm", new ActionCharm())
        {
        }
    }

    #endregion
}