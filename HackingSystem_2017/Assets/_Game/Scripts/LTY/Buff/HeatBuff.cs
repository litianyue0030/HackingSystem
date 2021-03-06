﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;
using HackingSystem.Tools;

namespace LTY
{
    public class HeatBuff : Buff
    {
        private int m_heatDelta;
        private Timer m_timer;
        public HeatBuff(Bot source, float time,BuffType buffType, BuffSystem owner,int heatDelta)
        {
            this.Source = source;
            this.Owner = owner;
            this.BuffType = buffType;
            m_heatDelta = heatDelta;
            m_timer = new Timer();
            BuffEndRule = new RuleTimeOver(time);
        }

        public override void Enter()
        {
            m_timer.SetExpiredTime(Time.time + 1);
        }

        public override void Execute()
        {
            Debug.Log("Heat Excute");
            if (m_timer.IsExpired(Time.time))
            {
                Owner.owner.Abilities.Heat.Value += m_heatDelta;
                Debug.Log("Heat: " + Owner.owner.Abilities.Heat.Value);
                m_timer.SetExpiredTime(Time.time + 1);
            }
        }

        public override void Exit()
        {
        }
    }
}