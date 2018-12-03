using UnityEngine;
using UnityEditor;
using System;

namespace HackingSystem
{

    public class Abilities
    {
        public Bot owner { get; set; }

        StatePoint hp;

        public StatePoint HP
        {
            get { return hp; }
        }

        StatePoint mp;

        public StatePoint MP
        {
            get { return mp; }
        }

        int gp;
        ///<summary>不可回复的护盾</summary>
        public int GPInrecovable
        {
            get { return gp; }
            set
            {
                if (value <= 0)
                {
                    gp = 0;
                    hp.Value += value;
                    return;
                }
                gp = value;
            }
        }

        float HeatMinusTime;

        StatePoint heat;
        public StatePoint Heat
        {
            get { return heat; }
        }

        float moveSpeedRate = 1.0f;
        public float MoveSpeedRate
        {
            get { return moveSpeedRate; }
            set
            {
                if (value <= 0)
                {
                    Debug.LogError("移速系数必须乘算，且不能为0或低于0");
                }
                moveSpeedRate = value;
            }
        }
        
        //KnockBackDefend
        public int KnockBackDefend
        {
            get; set;
        }

        /// <summary>
        /// 攻击时的伤害乘算（N倍伤害），乘算
        /// </summary>
        public virtual float power
        {
            get;
            set;
        }

        //ActionRate
        float actionRate = 1.0f;
        public float ActionRate
        {
            get { return actionRate; }
            set
            {
                if (value <= 0)
                {
                    Debug.LogError("动作速度不能为0或为负，只能被乘算");
                }
                actionRate = value;
            }
        }

        //定额减伤
        int strongGP;
        /// <summary>
        /// 定额减伤数值
        /// </summary>
        public int StrongGP
        {
            get { return strongGP; }
            set
            {
                strongGP = value;
            }
        }

        //DamageInComeRate 受到伤害的比例
        float damageInComeRate;
        public float DamageInComeRate
        {
            get { return damageInComeRate; }
            set
            {
                if (value <= 0)
                {
                    Debug.LogError("受伤不能为0或者小于0，且必须乘算");
                }
                damageInComeRate = value;
            }
        }

        public Abilities(Bot owner)
        {
            this.owner = owner;
            this.hp = new StatePoint(1);
            this.mp = new StatePoint(1);
            DataInitialization();
        }

        private void DataInitialization()
        {
            
            heat = new StatePoint(0, 100);
            power = 1;
            hp.OnValueChange += Hp_OnValueChange;
            heat.OnValueChange += Heat_OnValueChange;
        }

        public void Refresh()
        {
            HeatMinusTime -= Time.deltaTime;
            if (HeatMinusTime<=0)
            {
                HeatMinusTime += 0.03f;
                heat.Value--;
            }
        }

        private void Heat_OnValueChange(object sender, ValueChingingEventArgs<int> e)
        {
            if (e.NewValue > e.OldValue)
            {
                HeatMinusTime = 2f;
            }
        }

        private void Hp_OnValueChange(object sender, ValueChingingEventArgs<int> e)
        {
            if (e.NewValue <= 0)
            {
                owner.OnLifeZero();
            }
        }

        public Abilities(Bot owner,int hp,int mp)
        {
            this.hp = new StatePoint(hp);
            this.mp = new StatePoint(mp);
            this.owner = owner;
            DataInitialization();
        }
        
    }

    public class ValueChingingEventArgs<T>:EventArgs
    {
        public ValueChingingEventArgs(T oldValue,T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
            cancelChange = false;
        }

        public readonly T OldValue;
        public T NewValue;
        bool cancelChange;
        public bool CancelChange
        {
            get { return cancelChange; }
            set { cancelChange = value | cancelChange; }
        }
    }

    /// <summary>
    /// 状态条，拥有一个最大值和最小值0
    /// </summary>
    public class StatePoint
    {
        public event EventHandler<ValueChingingEventArgs<int>> OnValueChange;

        public StatePoint(int value)
        {
            curValue = maxValue = value;
        }

        public StatePoint(int curValue,int maxValue)
        {
            this.curValue = curValue;
            this.maxValue = maxValue;
        }

        int maxValue;
        public int MaxValue {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                if (value < curValue)
                {
                    curValue = value;
                }
            }
        }
        int curValue;
        
        public int Value
        {
            get { return curValue; }
            set
            {
                var e = new ValueChingingEventArgs<int>(curValue, value);
                OnValueChange(this, e);
                value = e.NewValue;
                if (e.CancelChange)
                {
                    return;
                }
                if (value>maxValue)
                {
                    value = maxValue;
                }
                else if (value <0)
                {
                    value = 0;
                }
                curValue = value;
            }
        }
        public static implicit operator int(StatePoint p)
        {
            return p.curValue;
        }
    }
}