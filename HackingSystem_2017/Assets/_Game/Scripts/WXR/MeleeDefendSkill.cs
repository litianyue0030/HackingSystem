using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace HackingSystem.Melee
{
    /// <summary>
    /// 近战类技能：防御
    /// 使用时在面前形成一面圆形盾牌，可以阻挡或者减少穿过盾的伤害。
    /// 
    /// 同类技能：HackingSystem.Dustbin.SkillSheldMode
    /// </summary>
    public class MeleeDefendSkill : Skill
    {
        Animator a;
        Vector2 diration;
        Rigidbody ownerRigid;
        string pressKey;
        public static GameObject modelPrefab;
        public GameObject model;
        Transform eriyaTrans;
        Transform modelInstall;
        float damageDeclineRatio; // 减伤折扣指数：0~1之间的小数 伤害将乘以这个系数 系数越小，减伤幅度越大
        float shieldAngle; // 保护半角：和盾面夹角在这个范围内的攻击被认为穿过盾，伤害减少 角度越大，保护范围越大

        static MeleeDefendSkill()
        {
            modelPrefab = Resources.Load<GameObject>("WXR/shieldPrefab");
        }

        public override Animator anim
        {
            get { return a; }
            set
            {
                a = value;
                SetRule();
                eriyaTrans = owner.owner.Owner.transform;
                modelInstall = eriyaTrans.GetChild(2);
            }
        }

        public void SetRule()
        {
            pressKey = "1"; //按住持盾（按下开盾，抬起收盾）
            damageDeclineRatio = 0.5f;
            shieldAngle = 60;
            CurrentPhase = SkillActionPhase.InActive;
            RuleCast = new KeyDown(pressKey); //TODO: 检查人物姿势和状态，替换为具体部位Control类接口
            RuleComplete = new KeyUp(pressKey); //TODO: 检查人物姿势和状态，替换为具体部位Control类接口
            RuleEnd = new RuleTimeOver(0.2f);
        }

        public override void Enter()
        {
            ownerRigid = owner.owner.Owner.GetComponent<Rigidbody>();
            diration = owner.owner.Owner.Diration2;
            model = GameObject.Instantiate(modelPrefab, modelInstall);
            // TODO: 加入开盾动画
            shielded = false;
        }

        private void Owner_OnBeingDamage(object sender, DamageEventArgs<Bot> e)
        {
            // 受到和盾面夹角在保护半角范围内的攻击被认为穿过盾，伤害减少
            // TODO: 以玩家中心为坐标计算点，如果攻击不同部位伤害不同，此处应修改成具体被攻击部位的坐标
            if (Vector3.Angle(e.AttackPosition - eriyaTrans.position, eriyaTrans.forward) < shieldAngle)
            {
                e.Damage = (int)(e.Damage * damageDeclineRatio);
            }
        }

        bool shielded; // 记录是否已经叠加盾效果，用于中途退出时检查是否要消除盾效果

        public override void Casting()
        {
            owner.owner.Owner.BattleSystem.OnBeingDamage += Owner_OnBeingDamage;
            shielded = true;
        }

        public override void Exit()
        {

        }

        public override void Complete()
        {
            // TODO: 加入收盾动画
        }

        public override void AfterComplete()
        {
            GameObject.Destroy(model);
            if (shielded)
            {
                owner.owner.Owner.BattleSystem.OnBeingDamage -= Owner_OnBeingDamage;
                shielded = false;
            }
        }

        public override void Decast()
        {
            // TODO: 加入收盾动画
            GameObject.Destroy(model);
            if (shielded)
            {
                owner.owner.Owner.BattleSystem.OnBeingDamage -= Owner_OnBeingDamage;
                shielded = false;
            }
        }

        public override void InActive()
        {

        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class KeyDown : ExecuteRule
    {
        string pressKey;
        public KeyDown(string pressKey)
        {
            this.pressKey = pressKey;
        }
               
        public override bool RuleExecute()
        {
            // TODO: 用Control类中的身体部位技能接口替换
            // 如：EnergyBagArrowDown()
            return Input.GetKeyDown(pressKey.ToLower()) || Input.GetKeyDown(pressKey.ToUpper());
        }
    }

    public class KeyUp : ExecuteRule
    {
        string pressKey;
        public KeyUp(string pressKey)
        {
            this.pressKey = pressKey;
        }

        public override bool RuleExecute()
        {
            // TODO: 用Control类中的身体部位技能接口替换
            // 如：EnergyBagArrowUp()
            return Input.GetKeyUp(pressKey.ToLower()) || Input.GetKeyUp(pressKey.ToUpper());
        }
    }
}