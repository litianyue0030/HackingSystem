using UnityEngine;
using UnityEditor;
namespace HackingSystem
{
    public class BattleSystem : ScriptableObject
    {
        Bot owner;
        public Bot Owner
        {
            get { return owner; }
        }
        public BattleSystem(Bot owner) { this.owner = owner; }

        public static Vector3 AttackPosition_NaN
        {
            get
            {
                return new Vector3(float.NaN, float.NaN, float.NaN);
            }
        }

        public static bool IsAttackPosition_NaN(Vector3 AttackPosition)
        {
            return float.IsNaN(AttackPosition.x) || float.IsNaN(AttackPosition.y) || float.IsNaN(AttackPosition.z);
        }

        /// <summary>
        /// 攻击受到技能伤害
        /// </summary>
        /// <param name="Damage">伤害量（初始值）</param>
        /// <param name="damageMode">伤害类型</param>
        /// <param name="source">伤害来源</param>
        /// <param name="HitValue">硬直量</param>
        /// <param name="AttackDiration">攻击方向,为0向量时为没有攻击方向,该向量为0向量判定为没有攻击方向</param>
        /// <param name="AttackPosition">攻击身位，自身中心为原点,该向量出现float.NaN判定为没有攻击身位的技能</param>
        /// <param name="StrongGPImmuse">是否无视强护甲，无视时，即便强护甲为负数带来的增伤也会无视</param>
        /// <param name="DamageRateImmuse">是否无视收到伤害按比例增加/减少机制</param>
        public void BeDamageBySkill(int Damage, Skill source, DamageMode damageMode, int HitValue, Vector3 AttackDiration, Vector3 AttackPosition, bool DownDamage = false, bool StrongGPImmuse = false, bool DamageRateImmuse = false)
        {
            DamageInfo<Skill> info = new DamageInfo<Skill>();
            info.Damage = Damage;
            info.source = source;
            info.damageMode = damageMode;
            info.HitValue = HitValue;
            info.AttackDirection = AttackDiration;
            info.AttackPosition = AttackPosition;
            info.DownDamage = DownDamage;
            info.StrongGPImmuse = StrongGPImmuse;
            info.DamageRateImmuse = DamageRateImmuse;

            BeDamageBySkill(info);
        }

        public void BeDamageBySkill(DamageInfo<Skill> info)
        {
            DamageEventArgs<Bot> e = new DamageEventArgs<Bot>(info.Damage, info.source.owner.owner.Owner, info.damageMode, info.HitValue, info.AttackDirection, info.AttackPosition, info.StrongGPImmuse, info.DamageRateImmuse, info.DownDamage);
            if (OnBeingDamage != null)
            {
                OnBeingDamage(this, e);
            }
            DamageEventArgs<Skill> e1 = new DamageEventArgs<Skill>(e.Damage, info.source, e.DamageMode, e.HitValue, info.AttackDirection, info.AttackPosition, e.StrongGPImmuse, e.DamageRateImmuse, info.DownDamage);
            e1.Cancel = e.Cancel;
            if (OnBeingDamageBySkill != null)
            {
                OnBeingDamageBySkill(this, e1);
            }

            //伤害计算前的事件
            info.Damage = e1.Damage;
            info.damageMode = e1.DamageMode;
            info.HitValue = e1.HitValue;
            if (e1.Cancel)
            {
                return;
            }
            //百分比减伤
            info.Damage = (int)(owner.Abilities.DamageInComeRate * info.Damage);

            //强护甲减伤
            info.Damage -= owner.Abilities.StrongGP;

            //扣血
            owner.Abilities.HP.Value -= info.Damage;
            //硬直计算 时间=0.3*技能硬直/对方人物硬直(0.6秒上限)，如果比值超过2就击飞，处于被打出硬直中的敌人，只要技能硬直不为0，就会刷新硬直，时间至少0.3s
            KnockBackCalc(info.source.owner.owner.Owner, info.HitValue, info.AttackDirection);
        }

        private void KnockBackCalc(Bot source, int HitValue, Vector3 AttackDiration)
        {
            float HitTime = 0;
            bool Hit2 = false;
            KnockBackType t = KnockBackType.KnockOut;
            if (HitValue > 0)
            {
                AttackDiration.y = 0;
                AttackDiration.Normalize();
                if (owner.KnockBackInformation != null)
                {
                    Hit2 = true;
                    if (owner.KnockBackInformation.type != KnockBackType.None)
                    {
                        t = owner.KnockBackInformation.type;
                    }
                    HitTime = HitValue > owner.Abilities.KnockBackDefend ? HitValue / owner.Abilities.KnockBackDefend * 0.3f : 0.3f;
                }
                else if (HitValue > owner.Abilities.KnockBackDefend)
                {
                    HitTime = HitValue / owner.Abilities.KnockBackDefend * 0.3f;
                }
                else
                {
                    return;
                }
                if (owner.OnAir)
                {
                    t = KnockBackType.BlastOut;
                    AttackDiration.y = 3;
                }
                if (HitTime > 0.6f)
                {
                    HitTime = -1;
                }
                if (OnKnockBack != null)
                {
                    KnockBackEventArgs e = new KnockBackEventArgs(HitTime, t, Hit2);
                    e.KnockOutVelocity = new Vector2(1, 0);
                    AttackDiration.y = e.KnockOutVelocity.y;
                    AttackDiration.x *= e.KnockOutVelocity.x;
                    AttackDiration.z *= e.KnockOutVelocity.x;
                    if (e.Cancel)
                    {
                        return;
                    }
                    HitTime = e.HitTime;
                    t = e.type;
                }
                Buff b = null;
                switch (t)
                {
                    case KnockBackType.KnockOut:
                        owner.BuffSystem.AddBuff(b = new BuffKnockOver(source, HitTime, AttackDiration));
                        break;
                    case KnockBackType.None:
                        owner.BuffSystem.AddBuff(b = new BuffKnockOver(source, HitTime, Vector3.zero));
                        break;
                    case KnockBackType.BlastOut:
                        owner.BuffSystem.AddBuff(b = new BuffKnockOut(source, AttackDiration));
                        break;
                    case KnockBackType.Ground:
                        owner.BuffSystem.AddBuff(b = new BuffDownGround(source));
                        break;
                    default:
                        break;
                }
                owner.KnockBackInformation = new KnockBackInformation();
                owner.KnockBackInformation.type = t;
                owner.KnockBackInformation.Time = HitTime;
                owner.KnockBackInformation.KnockBuff = b;
                owner.HitAnimExecute();
            }
            #region
            /*
            if (HitValue > 0)
            {
                //攻击硬直中敌人造成的二次硬直
                if (KnockBackTime > 0)
                {
                    float KnockBackTime_C = HitValue / KnockBackDefend;
                    if (KnockBackTime_C < 0.3f)
                    {
                        KnockBackTime_C = 0.3f;
                        KnockBackEventArgs e = new KnockBackEventArgs(KnockBackTime_C, KnockBackType.KnockOut, true);

                        if (KnockBackTime_C > KnockBackTime)
                        {
                            KnockBackTime = KnockBackTime_C;
                            //硬直动画重置
                            HitAnimExecute();
                        }
                    }
                }
                else if (HitValue > KnockBackDefend)
                {
                    float KnockBackTime_C = HitValue / KnockBackDefend;
                    if (KnockBackTime_C > 0.9f)
                    {
                        //击飞
                    }
                    else if (KnockBackTime_C > 0.6f)
                    {
                        KnockBackTime_C = 0.6f;
                    }
                    knockBacktime = KnockBackTime_C;
                    //硬直动画重置
                }
            }
            */
            #endregion
        }
        
        public event DamageEventHandler<Skill> OnBeingDamageBySkill;

        public event DamageEventHandler<Item> OnBeingDamageByItem;

        /// <summary>
        /// 收到非Buff来源的伤害时触发
        /// </summary>
        public event DamageEventHandler<Bot> OnBeingDamage;

        public event DamageBuffEventHandler OnBeingDamageByBuff;

        public event HealEventHandler OnHealing;

        /// <summary>
        /// 僵直时触发
        /// </summary>
        public event KnockBackEventHandler OnKnockBack;

        /// <summary>
        /// 攻击受到物件伤害
        /// </summary>
        /// <param name="Damage">伤害量（初始值）</param>
        /// <param name="damageMode">伤害类型</param>
        /// <param name="source">伤害来源</param>
        /// <param name="HitValue">硬直量</param>
        /// <param name="AttackDiration">攻击方向,为0向量时为没有攻击方向,该向量为0向量判定为没有攻击方向</param>
        /// <param name="AttackPosition">攻击身位，自身中心为原点,该向量出现float.NaN判定为没有攻击身位的技能</param>
        /// <param name="StrongGPImmuse">是否无视强护甲，无视时，即便强护甲为负数带来的增伤也会无视</param>
        /// <param name="DamageRateImmuse">是否无视收到伤害按比例增加/减少机制</param>
        public void BeDamageByItem(int Damage, Item source, DamageMode damageMode, int HitValue, Vector3 AttackDiration, Vector3 AttackPosition, bool downDamage = false, bool StrongGPImmuse = false, bool DamageRateImmuse = false)
        {
            DamageInfo<Item> info = new DamageInfo<Item>();
            info.Damage = Damage;
            info.source = source;
            info.damageMode = damageMode;
            info.HitValue = HitValue;
            info.AttackDirection = AttackDiration;
            info.AttackPosition = AttackPosition;
            info.DownDamage = downDamage;
            info.StrongGPImmuse = StrongGPImmuse;
            info.DamageRateImmuse = DamageRateImmuse;

            BeDamageByItem(info);
        }

        public void BeDamageByItem(DamageInfo<Item> info)
        {
            DamageEventArgs<Bot> e = new DamageEventArgs<Bot>(info.Damage, info.source.Owner.Owner, info.damageMode, info.HitValue, info.AttackDirection, info.AttackPosition, info.StrongGPImmuse, info.DamageRateImmuse, info.DownDamage);
            if (OnBeingDamage != null)
            {
                OnBeingDamage(this, e);
            }
            DamageEventArgs<Item> e1 = new DamageEventArgs<Item>(e.Damage, info.source, e.DamageMode, e.HitValue, info.AttackDirection, info.AttackPosition, e.StrongGPImmuse, e.DamageRateImmuse, info.DownDamage);
            e1.Cancel = e.Cancel;
            if (OnBeingDamageBySkill != null)
            {
                OnBeingDamageByItem(this, e1);
            }

            //伤害计算前的事件
            info.Damage = e1.Damage;
            info.damageMode = e1.DamageMode;
            info.HitValue = e1.HitValue;
            if (e1.Cancel)
            {
                return;
            }

            //强护甲减伤
            info.Damage -= owner.Abilities.StrongGP;

            //百分比减伤
            info.Damage = (int)(owner.Abilities.DamageInComeRate * info.Damage);

            //扣血
            owner.Abilities.HP.Value -= info.Damage;
            //硬直计算
            KnockBackCalc(info.source.Owner.Owner, info.HitValue, info.AttackDirection);
        }

        /// <summary>
        /// 减益状态收到伤害
        /// </summary>
        /// <param name="Damage">伤害量</param>
        /// <param name="source">伤害来源</param>
        /// <param name="damageMode">伤害类型</param>
        public void BeDamageOverBuff(int Damage, Buff source, DamageMode damageMode)
        {
            if (OnBeingDamageByBuff != null)
            {
                DamageBuffEventArgs e = new DamageBuffEventArgs(damageMode, Damage, source);
                OnBeingDamageByBuff(this, e);
                Damage = e.Damage;
                damageMode = e.DamageMode;
            }
            //扣血
            owner.Abilities.HP.Value -= Damage;
        }

        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="healValue">治疗值</param>
        /// <param name="healMode">治疗类型</param>
        /// <param name="HealSource">治疗源</param>
        public void BeHeal(int healValue, HealMode healMode, Bot HealSource)
        {
            if (OnHealing != null)
            {
                HealEventArgs e = new HealEventArgs(HealSource, healValue, healMode);
                healValue = e.HealValue;
                HealSource = e.HealSource;
                healMode = e.HealMode;
            }
        }
    }
}