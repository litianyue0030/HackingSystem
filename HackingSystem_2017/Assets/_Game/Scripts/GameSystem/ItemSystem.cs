
using UnityEngine;
using UnityEditor;
namespace HackingSystem
{
    public class Bag
    {
        public bool IsAutomata = false;
        public Bot Owner { get; set; }
        public HPBag[] HPBags = new HPBag[3];
        public GPBag[] GPBags = new GPBag[3];
        public DefaultItem[] items = new DefaultItem[3];
        public WeaponReader[] weaponReaders = new WeaponReader[7];
        public Item ItemUsing
        {
            get;set;
        }

        public void ItemUse()
        { 
            //使用道具，未实现
        }
    }

    public class RuleNoUsing:ExecuteRule
    {
        Bag owner;

        public RuleNoUsing(Bag owner)
        {
            this.owner = owner;
        }

        public override bool RuleExecute()
        {
            return owner.ItemUsing == null;
        }
    }

    public abstract class Item
    {
        public static int ItemidTicker = 0;

        public Bag Owner { get; set; }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public GameObject Icon { get; private set; }

        public Item(int id, string name, string description, GameObject icon)
        {

            this.Id = id;

            this.Name = name;

            this.Description = description;

            this.Icon = icon;

        }

        public ExecuteRule RuleUse { get; set; }

        public ExecuteRule RuleUseComplete { get; set; }

        public ExecuteRule RuleItemCancel { get; set; }
        
        /// <summary>
        /// 物品使用
        /// </summary>
        public abstract void ItemUse();

        /// <summary>
        /// 物品使用中
        /// </summary>
        public abstract void ItemUsing();

        /// <summary>
        /// 物品使用完毕
        /// </summary>
        public abstract void ItemUsed();

        /// <summary>
        /// 物品使用被取消
        /// </summary>
        public abstract void ItemCancelled();

        /// <summary>
        /// 物品非使用时刷新
        /// </summary>
        public abstract void InActive();
    }

    public abstract class DefaultItem : Item
    {
        public DefaultItem(int id, string name, string description, GameObject icon) : base(id, name, description, icon)
        {
        }
    }

    public class GPBag : Item
    {
        public static int itemid;
        static GPBag()
        {
            itemid = ItemidTicker;
            ItemidTicker++;
        }

        public GPBag() : base(itemid, "护甲包", "护甲包可以提供护甲", null)
        {
            RuleItemCancel = new RuleFalse();
            RuleUse = new RuleSkillInActive(Owner.Owner) & new RuleNoUsing(Owner);
            RuleUseComplete = new RuleTimeOver(4);
        }

        public override void InActive()
        {
            
        }

        public override void ItemCancelled()
        {
        }

        public override void ItemUse()
        {
        }

        public override void ItemUsed()
        {
            Owner.Owner.Abilities.GPInrecovable = 40;
        }

        public override void ItemUsing()
        {
        }
    }

    public class HPBag : Item
    {
        public static int itemid;
        
        static HPBag()
        {
            itemid = ItemidTicker;
            ItemidTicker++;
        }

        public int ListID { get; set; }

        public int RecovTick = 10;

        public float RecovDeltaTime = 1;

        public float totalTime = 5;

        /// <summary>
        /// 构造医药包
        /// </summary>
        /// <param name="RecovTick">每一跳回复的HP</param>
        /// <param name="RecovDeltaTime">每一跳的间隔</param>
        /// <param name="RecovTime">总回复时间</param>
        public HPBag(int RecovTick, float RecovDeltaTime, float RecovTime) : base(itemid, "能量包", "能够回复机械能量的注射装置", null)
        {
            this.RecovTick = RecovTick;
            this.RecovDeltaTime = RecovDeltaTime;
            this.totalTime = RecovTime;
            RuleUse = new RuleSkillInActive(Owner.Owner) & new RuleNoUsing(Owner);
            RuleUseComplete = new RuleTimeOver(5);
            RuleItemCancel = new RuleFalse();
        }

        public override void ItemUse()
        {
            //读条
        }

        public override void ItemUsing()
        {
        }

        public override void ItemUsed()
        {
            Owner.Owner.BuffSystem.AddBuff(new BuffRecov(RecovTick,RecovDeltaTime,totalTime));
        }

        public override void ItemCancelled()
        {
        }

        public override void InActive()
        {
        }
    }

    public enum WeaponType
    {
        MainWeapon,
        SubWeapon,
        CoreWeapon,
        BackWeapon
    }

    /// <summary>
    /// 机械人偶不能使用武器图纸
    /// </summary>
    public abstract class WeaponReader:Item
    {
        Weapon weapon;

        public WeaponReader(int id, string name, string description, GameObject icon) : base(id, name, description, icon)
        {
        }
    }

    public class BuffRecov : Buff
    {
        int recovTick;
        float RecovDeltaTime;
        float TRecovDeltaTime;
        public BuffRecov(int RecovTick,float RecovDeltaTime,float totalTime)
        {
            recovTick = 10;
            TRecovDeltaTime = RecovDeltaTime;
            BuffEndRule = new RuleTimeOver(totalTime);
            this.BuffType = BuffType.buff;
        }

        public override void Enter()
        {
            
        }

        public override void Execute()
        {
            RecovDeltaTime -= Time.deltaTime;
            if (RecovDeltaTime<0)
            {
                RecovDeltaTime += TRecovDeltaTime;
                Owner.owner.BattleSystem.BeHeal(10, HealMode.EnergyBag, Owner.owner);
            }
        }

        public override void Exit()
        {
        }
    }
}