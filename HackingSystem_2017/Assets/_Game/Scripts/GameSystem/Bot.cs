using UnityEngine;
namespace HackingSystem
{
    /// <summary>
    /// 关于处理伤害计算事件
    /// </summary>
    /// <typeparam name="T">伤害来源的类型</typeparam>
    /// <param name="sender">事件发起者</param>
    /// <param name="e">参数</param>
    public delegate void DamageEventHandler<T>(object sender, DamageEventArgs<T> e);

    public delegate void HealEventHandler(object sender, HealEventArgs e);
    /// <summary>
    /// 用于处理Buff伤害的事件
    /// </summary>
    /// <param name="sender">发起者</param>
    /// <param name="e">参数</param>
    public delegate void DamageBuffEventHandler(object sender, DamageBuffEventArgs e);

    /// <summary>
    /// 用于处理硬直结算的事件
    /// </summary>
    /// <param name="sender">发起者</param>
    /// <param name="e">参数</param>
    public delegate void KnockBackEventHandler(object sender, KnockBackEventArgs e);

    /// <summary>
    /// 关于处理伤害计算的事件的参数
    /// </summary>
    /// <typeparam name="T">伤害来源的类型</typeparam>
    public class DamageEventArgs<T>
    {
        event HealEventHandler h;
        void hd(object sender, HealEventArgs e)
        {
            Debug.Log("A");
        }
        public DamageEventArgs(int damage, T source, DamageMode damageMode, int HitValue, Vector3 AttackDiration, Vector3 AttackPosition, bool StrongGPImmuse, bool DamageRateImmuse, bool DownDamage)
        {
            h += hd;
            h(null, null);
            this.Damage = damage;
            this.source = source;
            this.DamageMode = damageMode;
            this.HitValue = HitValue;
            this.attackDiration = AttackDiration;
            this.attackPosition = AttackPosition;
            this.strongGPImmuse = StrongGPImmuse;
            this.damageRateImmuse = DamageRateImmuse;
            cancel = false;
            this.DownDamage = DownDamage;
        }
        bool cancel;
        bool downDamage;

        /// <summary>
        /// 倒地无视只能激活，不能取消
        /// </summary>
        public bool DownDamage
        {
            get { return downDamage; }
            set
            {
                downDamage = downDamage || value;
            }
        }

        /// <summary>
        /// 攻击取消只能激活，不能从激活状态取消
        /// </summary>
        public bool Cancel
        { get { return cancel; } set { cancel = value || cancel; } }
        int damage;
        public int Damage
        {
            get { return damage; }
            set
            {
                if (value > 0)
                {
                    damage = value;
                }
                else
                {
                    damage = 0;
                }
            }
        }
        public DamageMode DamageMode { get; set; }
        bool cric;



        public int HitValue { get; set; }
        Vector3 attackDiration;
        public Vector3 AttackDiration { get { return attackDiration; } }
        Vector3 attackPosition;
        public Vector3 AttackPosition { get { return attackPosition; } }
        bool strongGPImmuse;
        /// <summary>
        /// 强护甲免疫只能激活，不能从激活状态下取消
        /// </summary>
        public bool StrongGPImmuse { get { return strongGPImmuse; } set { strongGPImmuse = value || strongGPImmuse; } }

        bool damageRateImmuse;
        /// <summary>
        /// 伤害按比例改变免疫只能激活，不能从激活状态下取消
        /// </summary>
        public bool DamageRateImmuse { get { return damageRateImmuse; } set { damageRateImmuse = value || damageRateImmuse; } }

        public T source { get; private set; }
    }

    public class DamageBuffEventArgs
    {
        public DamageBuffEventArgs(DamageMode mode, int Damage, Buff Source)
        {
            DamageMode = mode;
            this.Damage = Damage;
            this.Source = Source;
        }
        public DamageMode DamageMode { get; set; }
        public int Damage { get; set; }
        public Buff Source { get; private set; }
    }

    public class HealEventArgs
    {
        bool cancel;
        public HealEventArgs(Bot source, int value, HealMode mode)
        {
            cancel = false;
            HealSource = source;
            HealMode = mode;
            HealValue = value;
        }
        /// <summary>
        /// 治疗取消只能激活，不能从激活状态取消
        /// </summary>
        public bool Cancel
        { get { return cancel; } set { cancel = value || cancel; } }
        public int HealValue
        { get; set; }
        public HealMode HealMode { get; set; }

        public Bot HealSource { get; private set; }
    }

    public class AttackResult
    {
        public bool Cancelled { get; private set; }
        public int FinalDamage { get; private set; }
        public bool Cric { get; private set; }
        public bool KnockBack { get; private set; }
        public float KnockTime { get; private set; }
    }

    public enum KnockBackType
    {
        /// <summary>
        /// 最普通的硬直类型，有轻微击退
        /// </summary>
        KnockOut,
        /// <summary>
        /// 没有击退的硬直类型
        /// </summary>
        None,
        /// <summary>
        /// 击飞型，飞到空中，到达地面会造成倒地，此类型硬直没有持续时间
        /// </summary>
        BlastOut,
        /// <summary>
        /// 倒地型，造成倒地
        /// </summary>
        Ground
    }

    public class KnockBackEventArgs
    {
        public KnockBackEventArgs(float hitTime, KnockBackType knockBackType,bool KnockBack2)
        {
            HitTime = hitTime;
            type = knockBackType;
            this.KnockBack2 = KnockBack2;
        }
        bool cancel = false;
        public bool Cancel
        {
            get { return cancel; }
            set { Cancel = cancel || value; }
        }
        public float HitTime
        {
            get; set;
        }
        public KnockBackType type
        {
            get; set;
        }
        /// <summary>
        /// 硬直带来击退的速度，（平面方向速率，y轴速度）
        /// </summary>
        public Vector2 KnockOutVelocity
        {
            get;set;
        }
        /// <summary>
        /// 攻击硬直中敌人造成的2次硬直
        /// </summary>
        public bool KnockBack2
        {
            get;private set;
        }
    }

    public class KnockBackInformation
    {
        public KnockBackType type;
        /// <summary>
        /// 硬直时间仅在非击飞硬直有效
        /// </summary>
        public float Time;
        /// <summary>
        /// 硬直造成的对应的Debuff
        /// </summary>
        public Buff KnockBuff;
    }

    public class DamageInfo<T> where T:class
    {
        /// <summary>
        /// 伤害量
        /// </summary>
        public int Damage = 0;
        /// <summary>
        /// 来源
        /// </summary>
        public T source = null;
        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageMode damageMode = DamageMode.Normal;
        /// <summary>
        /// 僵直造成
        /// </summary>
        public int HitValue = 0;
        /// <summary>
        /// 攻击方向
        /// </summary>
        public Vector3 AttackDirection = Vector3.zero;
        /// <summary>
        /// 攻击位置
        /// </summary>
        public Vector3 AttackPosition = BattleSystem.AttackPosition_NaN;
        /// <summary>
        /// 能否对倒地单位造成伤害
        /// </summary>
        public bool DownDamage = false;
        /// <summary>
        /// 是否无视定额增减伤
        /// </summary>
        public bool StrongGPImmuse = false;
        /// <summary>
        /// 是否无视百分比增减伤
        /// </summary>
        public bool DamageRateImmuse = false;
    }

    public abstract class Bot : MonoBehaviour
    {
        /// <summary>
        /// 面向锁定，锁定面向时设定的面向不能改变，但是设置的面向会被保存，同时解锁的时候进行面向更新，需要自行实现
        /// </summary>
        public abstract bool DirectionLock { get; set; }

        /// <summary>
        /// 面向信息
        /// </summary>
        public abstract Vector3 Direction
        {
            get; set;
        }

        int interrupt;
        /// <summary>
        /// 打断，大于0时为打断中
        /// </summary>
        public int Interrupt
        {
            get
            {
                return interrupt;
            }
            set
            {
                interrupt = value;
                if (interrupt > 0)
                {
                    weaponSystem.DecastSkill();

                }
            }
        }

        /// <summary>
        /// 用角度表示的面向信息，（-高低角,顺时针平面旋转角）
        /// </summary>
        public abstract Vector2 AngleDirection
        { get; set; }

        public Vector2 Diration2
        {
            get { return new Vector2(Direction.x, Direction.z); }
            set { Direction = new Vector3(value.x, Direction.y, value.y); }
        }

        /// <summary>
        /// 请在这里实现角色移动模块
        /// </summary>
        public abstract void MoveFrame();
        
        protected Abilities abilities;

        /// <summary>
        /// 能力值
        /// </summary>
        public Abilities Abilities
        {
            get { return abilities; }
        }
        
        public abstract bool OnAir
        {
            get;
        }

        WeaponSystem weaponSystem;

        public WeaponSystem WeaponSystem
        {
            get { return weaponSystem; }
        }
        
        BattleSystem battleSystem;
        public BattleSystem BattleSystem
        {
            get { return battleSystem; }
        }
        public abstract void HitAnimExecute();
        
        public abstract Animator Animator
        {
            get;
        }

        //Bag(item)
        Bag bag;
        public Bag Bag
        {
            get { return bag; }
            set
            {
                bag = value;
                bag.Owner = this;
            }
        }

        KnockBackInformation knockBackInfo;
        public KnockBackInformation KnockBackInformation
        {
            get { return knockBackInfo; }
            set
            {
                knockBackInfo = value;
            }
        }

        //Buff
        BuffSystem buffSystem;
        public BuffSystem BuffSystem
        {
            get { return buffSystem; }
        }

        //Action
        GameAction<Bot> action;
        public GameAction<Bot> Action
        {
            get { return action; }
            set
            {
                if (action != null)
                {
                    action.Exit();
                }
                if (value!=null)
                {
                    value.Executor = this;
                    value.Enter();
                }
                action = value;
            }
        }

        public virtual void OnLifeZero()
        {
            
        }

        public virtual void Initialzation()
        {
            buffSystem = new BuffSystem();
            buffSystem.owner = this;
            battleSystem = new BattleSystem(this);
            weaponSystem = new WeaponSystem(this);
        }

        // Use this for initialization
        void Start()
        {
            Initialzation();
        }

        // Update is called once per frame
        void Update()
        {
            Refresh();
        }

        public virtual void Refresh()
        {
            abilities.Refresh();
            //Buff结算
            buffSystem.Update();
            //技能CD结算
            //技能
            weaponSystem.Refresh();
            //AI 
            if (Interrupt <= 0)
            {
                if (action != null)
                {
                    action.Execute();
                }
            }
            //动画
        }
    }

    public enum HealMode
    {
        EnergyBag
    }

    public enum DamageMode
    {
        Bullet,
        Normal,
        Blizzard,
        flame,
        thunder,
        Energy
    }
}
