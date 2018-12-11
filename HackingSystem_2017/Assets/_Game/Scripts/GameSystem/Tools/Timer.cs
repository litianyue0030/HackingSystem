using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackingSystem.Tools
{
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer
    {
        protected float m_ExpiredTime = 0;
        public void Reset()
        {
            m_ExpiredTime = -1;
        }

        public void SetExpiredTime(float expiredTime)
        {
            m_ExpiredTime = expiredTime;
        }

        public bool IsExpired(float gameTime)
        {
            return m_ExpiredTime >= 0 && gameTime >= m_ExpiredTime;
        }

        public float GetRemaingTime(float gameTime)
        {
            return Mathf.Max(0, m_ExpiredTime - gameTime);
        }

        public void Copy(Timer t)
        {
            m_ExpiredTime = t.m_ExpiredTime;
        }
    }
}