using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HackingSystem.Melee
{
    /// <summary>
    /// 近战类技能：劈砍
    /// 使用后朝角色正面X距离造成一个扇形攻击。
    /// 
    /// 同类技能：HackingSystem.Dustbin.SkillSheldAttackCom1()&SkillSheldAttackCom1()
    /// 已实现二连击、动画、伴随冲刺
    /// 伤害对象这边用的是真判定（方案一：在武器上用碰撞器的trigger来找伤害对象）见DamageJudgeAttackCom1类的实现
    /// 
    /// 如果按照方老师的要求要用扇形区域判定的话，因为没接口能获取所有玩家的位置，还是需要用到碰撞器？
    /// 方案二：直接使用扇形底面的立柱碰撞器
    /// 方案三：边长是扇形两倍半径的立方体碰撞器再算一下碰到的玩家是否在扇形区域内
    /// </summary>
    public class MeleeSlashSkill : MonoBehaviour
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
