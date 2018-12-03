using UnityEngine;
using System.Collections;

namespace HackingSystem
{
    /// <summary>
    /// 机械人偶
    /// </summary>
    public abstract class AutoMata : Bot
    {
        public string CodeName { get; protected set; }
    }
}