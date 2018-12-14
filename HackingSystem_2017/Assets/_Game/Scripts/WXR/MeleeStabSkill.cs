using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackingSystem.Melee
{
    /// <summary>
    /// 近战类技能：突刺
    /// 朝鼠标指向一个点进行X米的穿刺类攻击。
    /// 
    /// 是否支持刺穿？
    /// 支持：在武器上增加碰撞器来寻找伤害对象（可以找到所有接触到的对象）
    /// 不支持：使用Ray来寻找伤害对象（只能找到照到的第一个，也就是这个方向上距离最近的一个）
    /// </summary>
    public class MeleeStabSkill : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
