using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem.Dustbin
{
    public class Dustbin : EnemyAndResource
    {

        public override Vector3 Direction
        {
            get
            {
                Vector3 ro = transform.eulerAngles;
                return new Vector3(Mathf.Sin(ro.y) * Mathf.Cos(ro.x), -Mathf.Sin(ro.x), Mathf.Cos(ro.y) * Mathf.Cos(ro.x));
            }

            set
            {
                value.Normalize();
                //m,n=E
                Matrix4x4 m = Matrix4x4.identity;
                Matrix4x4 n = Matrix4x4.identity;
                //平面旋转
                Vector2 Diration2 = new Vector2(value.x, value.z).normalized;
                float sy = Diration2.x;
                float cy = Diration2.y;
                m.m00 = m.m22 = cy;
                m.m20 = -(m.m02 = sy);
                //高低旋转
                float sx = -value.y;
                float cx = value.x / Diration2.x;
                n.m22 = n.m11 = cx;
                m.m12 = -(m.m21 = sx);
                Matrix4x4 x = m * n;
                transform.rotation = x.rotation;
            }
        }

        public override Vector2 AngleDirection
        {
            get;set;
        }

        public override bool DirectionLock
        {get;set;}

        public override bool OnAir
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override Animator Animator
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override void HitAnimExecute()
        {
        }

        public override void Initialzation()
        {
            base.Initialzation();
            abilities = new Abilities(this, 100, 0);
            abilities.KnockBackDefend = 50000;
            weaponList.Add(new MainWeaponDusbinTop());
        }
        
        public override void MoveFrame()
        {

        }
    }

    #region 垃圾桶盖子主武器
    
    public class RuleAnimatorPhaseIn : ExecuteRule
    {
        public Animator a;
        string name;
        int layer;
        public RuleAnimatorPhaseIn(string name, Animator a, int layer)
        {
            this.a = a;
            this.name = name;
        }



        public override bool RuleExecute()
        {
            return a.GetCurrentAnimatorStateInfo(layer).IsName(name);
        }


    }

    public class MainWeaponDusbinTop : MainWeapon
    {
        public static GameObject Model;
        public GameObject curModel;
        Transform EriyaModel;
        Transform ModelInstall;
        public Animator a;
        static MainWeaponDusbinTop()
        {
            Model = Resources.Load<GameObject>("Dusbin/Dusbin_Top");
            RuleTimeOver t = new RuleTimeOver(10);
        }

        public MainWeaponDusbinTop():base()
        {
            List<Skill> binskills = new List<Skill>(4);//
            binskills.Add(new SkillSheldAttackCom1());
            binskills.Add(new SkillSheldMode());
            binskills.Add(new SkillSheldAttackCom2());

            foreach (var item in binskills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = binskills;
        }

        /// <summary>
        /// 该函数会在该武器的Owner发生改变的时候调用，非构造时调用
        /// </summary>
        public override void initialization()
        {
            if (curModel != null)
            {
                GameObject.Destroy(curModel);
                curModel = null;
            }
            EriyaModel = Owner.transform;
            //获取右手位置
            ModelInstall = EriyaModel.GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
            //生成模型
            GameObject Model = GameObject.Instantiate(MainWeaponDusbinTop.Model,ModelInstall);
            curModel = Model;
            if (((Eriya.Eriya)Owner).eriyaMode == Eriya.EriyaMode.Hacking)
            {
                //动画初始化
                ((Eriya.Eriya)Owner).eriyaMode = Eriya.EriyaMode.Bot;
            }
            a = Owner.Animator;
            foreach (var item in SkillSystem.skills)
            {
                item.anim = a;
            }
        }

        public float CombTime = 0;
        public bool LeftPressed = false;
        public override void Refresh()
        {
            base.Refresh();
            if (Owner.Interrupt == 0)
            {
                if (CombTime > 0)
                {
                    CombTime -= Time.deltaTime;
                    //施法队列
                    if (!LeftPressed && Control.MainWPAttackLDown())
                    {
                        LeftPressed = true;
                    }
                    if (LeftPressed && SkillSystem.SkillCasting == null)
                    {
                        SkillSystem.Cast(2);
                        CombTime = 0;
                        return;
                    }

                    if (CombTime <=0)
                    {
                        //连击时间过了回到原状态
                        a.PlayInFixedTime("Stand");
                    }
                }
                else if (Control.MainWPAttackLPress())
                {
                    SkillSystem.Cast(0);
                    LeftPressed = false;
                }
                if (Control.MainWPAttackRPress())
                {
                    SkillSystem.Cast(1);
                    LeftPressed = false;
                }
            }
        }

        public override void dispose()
        {
            GameObject.Destroy(curModel);
        }
    }

    public class SkillSheldAttackCom2 : Skill
    {
        Animator a;
        bool movAttack;
        float spd;
        Vector2 Diration;
        Rigidbody ownerRigid;
        DamageJudgeAttackCom2 j;
        public override Animator anim
        {
            get { return a; }
            set
            {
                a = value;
                SetRule();
            }
        }

        public void SetRule()
        {
            RuleComplete = new RuleTrue();
            RuleEnd = new RuleAnimatorPhaseIn("Stand", a, a.GetLayerIndex("Normal"));
            RuleCast = new RuleAnimatorPhaseIn("Dusbin_MW_SheldAttackCom1", a, a.GetLayerIndex("Normal"));
        }

        public override void AfterComplete()
        {
            if (movAttack)
            {
                spd -= Time.deltaTime * 60;
                if (spd <= 0)
                {
                    if (j!=null)
                    {
                        GameObject.Destroy(j);
                        j = null;
                    }
                    return;
                }
                ownerRigid.velocity = new Vector3(Diration.x * spd, 0, Diration.y * spd);
            }
        }

        public override void Casting()
        {
            
        }

        public override void Complete()
        {
            //判定
            j = ((MainWeaponDusbinTop)owner.owner).curModel.AddComponent<DamageJudgeAttackCom2>();
            j.owner = this;
            
        }

        public override void Decast()
        {

        }

        public override void Enter()
        {
            anim.PlayInFixedTime("Dusbin_MW_SheldAttackCom2");
            movAttack = true;
            ownerRigid = owner.owner.Owner.GetComponent<Rigidbody>();
            Diration = owner.owner.Owner.Diration2;
            owner.owner.Owner.DirectionLock = true;
            spd = 24;
        }

        public override void Exit()
        {
            if (j != null)
            {
                GameObject.Destroy(j);
                j = null;
            }
            owner.owner.Owner.DirectionLock = false;
        }

        public override void InActive()
        {
        }
    }

    public class DamageJudgeAttackCom1 : MonoBehaviour
    {
        public SkillSheldAttackCom1 owner;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && other.GetComponent<Bot>() != owner.owner.owner.Owner)
            {
                Bot b = other.GetComponent<Bot>();
                b.BattleSystem.BeDamageBySkill(40, owner, DamageMode.Normal, 250, owner.owner.owner.Owner.Direction, owner.owner.owner.Owner.transform.position - b.transform.position);
            }
        }
    }

    public class DamageJudgeAttackCom2 : MonoBehaviour
    {
        public SkillSheldAttackCom2 owner;
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && other.GetComponent<Bot>() != owner.owner.owner.Owner)
            {
                Bot b = other.GetComponent<Bot>();
                b.BattleSystem.OnKnockBack += B_OnKnockBack;
                b.BattleSystem.BeDamageBySkill(60, owner, DamageMode.Normal, 450, owner.owner.owner.Owner.Direction, owner.owner.owner.Owner.transform.position - b.transform.position);
                b.BattleSystem.OnKnockBack -= B_OnKnockBack;
            }
        }

        private void B_OnKnockBack(object sender, KnockBackEventArgs e)
        {
            if (e.KnockBack2)
            {
                e.KnockOutVelocity = new Vector2(5, 10);
                e.type = KnockBackType.BlastOut;
            }
            else
            {
                e.HitTime = 1.2f;
                e.KnockOutVelocity = new Vector2(8, 0);
            }
        }
    }

    public class SkillSheldAttackCom1 : Skill
    {
        Animator a;
        bool movAttack;
        float spd;
        Vector2 Diration;
        Rigidbody ownerRigid;
        DamageJudgeAttackCom1 j = null;
        public override Animator anim
        {
            get { return a; }
            set
            {
                a = value;
                SetRule();
            }
        }

        public void SetRule()
        {
            RuleComplete = new RuleTimeOver(0.3f);
            RuleEnd = new RuleTimeOver(0.5f);
            RuleCast = new RuleAnimatorPhaseIn("Stand", a, a.GetLayerIndex("Normal"));
        }

        public SkillSheldAttackCom1()
        {
            CurrentPhase = SkillActionPhase.InActive;
        }

        public override void AfterComplete()
        {
            if (movAttack)
            {
                spd -= Time.deltaTime * 18;
                if (spd <= 0)
                {
                    if (j!=null)
                    {
                        GameObject.Destroy(j);
                        j = null;
                    }
                    return;
                }
                ownerRigid.velocity = new Vector3(Diration.x * spd, 0, Diration.y * spd);
            }
        }

        public override void Casting()
        {
            if (movAttack)
            {
                ownerRigid.velocity = new Vector3(Diration.x * spd, 0, Diration.y * spd);
            }
        }

        public override void Complete()
        {
            //伤害判定
            j = ((MainWeaponDusbinTop)owner.owner).curModel.AddComponent<DamageJudgeAttackCom1>();
            j.owner = this;
            ((MainWeaponDusbinTop)owner.owner).CombTime = 1.2f;
            if (movAttack)
            {
                spd = 9f;
            }
        }

        public override void Decast()
        {
        }

        public override void Enter()
        {
            anim.PlayInFixedTime("Dusbin_MW_SheldAttackCom1");
            movAttack = true;
            spd = 5 * Input.GetAxis("Vertical");
            owner.owner.Owner.DirectionLock = true;
            Diration = owner.owner.Owner.Diration2;
            ownerRigid = owner.owner.Owner.GetComponent<Rigidbody>();
        }

        public override void Exit()
        {
            if (j!=null)
            {
                GameObject.Destroy(j);
                j = null;
            }
            owner.owner.Owner.DirectionLock = false;
            movAttack = false;
        }

        public override void InActive()
        {

        }
    }

    class RuleMouseRightUnPress : ExecuteRule
    {
        public override bool RuleExecute()
        {
            return !Control.MainWPAttackRPress();
        }
    }

    class SkillSheldMode : Skill
    {
        Animator a;
        public override Animator anim
        {
            get { return a; }
            set
            {
                a = value;
                SetRule();
            }
        }
        public SkillSheldMode()
        {
            CurrentPhase = SkillActionPhase.InActive;
            
        }

        public void SetRule()
        {
            RuleComplete = new RuleAnimatorPhaseIn("Dusbin_MW_SheldMode", a, a.GetLayerIndex("Normal"));
            RuleEnd = new RuleMouseRightUnPress();
            RuleCast = new RuleAnimatorPhaseIn("Stand", a, a.GetLayerIndex("Normal"));
        }

        public override void AfterComplete()
        {
        }

        public override void Casting()
        {
        }
        bool completed;
        public override void Complete()
        {
            owner.owner.Owner.BattleSystem.OnBeingDamage += Owner_OnBeingDamage;
            completed = true;
        }

        private void Owner_OnBeingDamage(object sender, DamageEventArgs<Bot> e)
        {
            //受到正面攻击
            if (!e.StrongGPImmuse && Vector3.Dot(e.AttackPosition - ((Bot)sender).transform.position, ((Bot)sender).Diration2) > 0)
            {
                e.Damage -= 40;
            }
        }

        public override void Decast()
        {
            a.SetTrigger("Decast");
        }

        public override void Enter()
        {
            a.PlayInFixedTime("Dusbin_MW_SheldMode_Prev");
            completed = false;
        }

        public override void Exit()
        {
            a.PlayInFixedTime("Dusbin_MW_SheldMode_End");
            if (completed)
            {
                owner.owner.Owner.BattleSystem.OnBeingDamage -= Owner_OnBeingDamage;
            }
        }

        public override void InActive()
        {

        }
    }

    #endregion

    #region 垃圾桶盖子副武器
    public class SubWeaponDusbinTop : SubWeapon
    {
        public Animator a;
        public static GameObject model;
        static SubWeaponDusbinTop()
        {
            model = Resources.Load<GameObject>("Dusbin/Dusbin_Top_SW");
        }
        public override void initialization()
        {
            a = Owner.Animator;
            SkillSystem.skills = new List<Skill>();
            var v = new SkillThrowWP();
            SkillSystem.skills.Add(v);
            v.owner = SkillSystem;
            v.CurModel = GameObject.Instantiate(model, Owner.transform.GetChild(1));
            v.b = Owner;
            //binskills[2] = new SkillBinMainRight();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (Owner.Interrupt == 0)
            {
                if (Control.SubWPAttackLDown() && Owner.WeaponSystem.skillCasting == null)
                {
                    SkillSystem.Cast(0);
                }
            }
            
        }

        public override void dispose()
        {
            GameObject.Destroy(((SkillThrowWP)SkillSystem.skills[0]).CurModel);
        }
    }
    public class RuleTopIn:ExecuteRule
    {
        public bool TopIn = true;
        public override bool RuleExecute()
        {
            return TopIn;
        }
    }

    public enum TopState
    {
        InBot,
        Running,
        Backing
    }
    public class SkillThrowWP : Skill
    {
        public GameObject CurModel;
        public Bot b;
        public TopState state = TopState.InBot;
        public RuleTopIn TopIn = new RuleTopIn();

        public SkillThrowWP()
        {
            RuleCast = TopIn;
            RuleComplete = new RuleTimeOver(0.4f);
            RuleEnd = new RuleFalse();
        }

        public override void AfterComplete()
        {
        }

        public override void Casting()
        {
        }

        /// <summary>
        /// 盖子脱离并飞出
        /// </summary>
        public override void Complete()
        {
            state = TopState.Running;
            TopIn.TopIn = false;

            CurModel.transform.SetParent(null);
            var v = CurModel.AddComponent<ModelActionRun>();
            v.owner = this;
            v.diration = b.Direction;
            var v2 = v.transform.GetChild(0).gameObject.AddComponent<ColliderJudgeRun>();
            v2.owner = this;
        }

        public override void Decast()
        {
        }

        public override void Enter()
        {
            var a = owner.owner.Owner.Animator;
            a.PlayInFixedTime("Dusbin_SW_ThrowWP");
            RuleEnd = new RuleAnimatorPhaseIn("Stand", a, a.GetLayerIndex("Normal"));
        }

        public override void Exit()
        {
        }

        public override void InActive()
        {
            if (Control.SubWPAttackRDown() && state == TopState.Running)
            {
                GameObject.Destroy(CurModel.GetComponent<ModelActionRun>());
                GameObject.Destroy(CurModel.transform.GetChild(0).GetComponent<ColliderJudgeRun>());
                ModelActionBK bk = CurModel.AddComponent<ModelActionBK>();
                ColliderJudgeBK bkj = CurModel.transform.GetChild(0).gameObject.AddComponent<ColliderJudgeBK>();
                bkj.owner = this;
                bk.owner = this;
            }
        }
        
        
    }

    /// <summary>
    /// 盖子回来时判定类
    /// </summary>
    public class ColliderJudgeBK:MonoBehaviour
    {
        public SkillThrowWP owner;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (other.GetComponent<Bot>() == owner.b)
                {
                    //归位
                    transform.parent.GetComponent<Rigidbody>().isKinematic = true;
                    transform.parent.SetParent(owner.b.transform.GetChild(1));
                    transform.parent.localPosition = new Vector3(-0.412f, 0.731f, -0.116f);
                    transform.parent.localEulerAngles = new Vector3(22.941f, -198.533f, -66.941f);
                    owner.state = TopState.InBot;
                    owner.TopIn.TopIn = true;
                    Destroy(transform.parent.GetComponent<ModelActionBK>());
                    Destroy(this);
                }
                else
                {
                    Bot b = other.GetComponent<Bot>();
                    b.BattleSystem.BeDamageBySkill(45, owner, DamageMode.Normal, 350, owner.b.transform.position - transform.position, transform.position - b.transform.position);

                }
            }
        }
    }
    /// <summary>
    /// 盖子回来时行为类
    /// </summary>
    public class ModelActionBK : MonoBehaviour
    {
        public SkillThrowWP owner;
        public float speed = 30;
        private void LateUpdate()
        {
            Vector3 Diration = owner.b.transform.position - transform.position;
            GetComponent<Rigidbody>().velocity = speed * Diration;
        }
    }
    /// <summary>
    /// 盖子弹出行为类
    /// </summary>
    public class ModelActionRun : MonoBehaviour
    {
        public SkillThrowWP owner;
        public Vector3 diration;
        float speed = 20;
        float time = 0;
        private void Start()
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        private void Update()
        {
            if (GameSystem.paused)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                time += Time.deltaTime;
                if (time >= 2)
                {
                    //时间超出直接收回
                    ModelActionBK bk = gameObject.AddComponent<ModelActionBK>();
                    ColliderJudgeBK bkj = transform.GetChild(0).gameObject.AddComponent<ColliderJudgeBK>();
                    bkj.owner = owner;
                    bk.owner = owner;
                    Destroy(this);
                    Destroy(transform.GetChild(0).GetComponent<ColliderJudgeRun>());
                    owner.state = TopState.Backing;
                }
            }
        }

        private void LateUpdate()
        {
            GetComponent<Rigidbody>().velocity = diration * speed;
        }
    }
    /// <summary>
    /// 盖子弹出判定类
    /// </summary>
    public class ColliderJudgeRun:MonoBehaviour
    {
        public SkillThrowWP owner;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && other.GetComponent<Bot>() != owner.b)
            {
                Bot b = other.GetComponent<Bot>();
                b.BattleSystem.BeDamageBySkill(25, owner, DamageMode.Normal, 250, owner.b.transform.position - transform.position, transform.position - b.transform.position);
                b.BuffSystem.AddBuff(new BuffSpeedLimitThrowSW(owner.b));
            }
        }
    }

    class BuffSpeedLimitThrowSW : Buff
    {
        public override BuffType buffType
        {
            get
            {
                return BuffType.debuff;
            }
        }

        public BuffSpeedLimitThrowSW(Bot source)
        {
            this.source = source;
            BuffEndRule = new RuleTimeOver(2);
        }
        float spd = 0.1f;
        float aspd = 0.45f;

        public override void Enter()
        {
            source.Abilities.MoveSpeedRate *= spd;
        }

        public override void Execute()
        {
            source.Abilities.MoveSpeedRate /= spd;
            spd += aspd * Time.deltaTime;
            source.Abilities.MoveSpeedRate *= spd;
        }

        public override void Exit()
        {
            source.Abilities.MoveSpeedRate /= spd;
        }
    }



    #endregion

    #region 垃圾桶盖子背部武器
    public class BackWeaponDusbinTop : BackWeapon
    {
        public static GameObject model;
        GameObject curModel;
        Transform EriyaModel;
        Transform ModelInstall;
        static BackWeaponDusbinTop()
        {
            model = Resources.Load<GameObject>("Dusbin/Dusbin_Top_BW");
        }
        public override void dispose()
        {
            GameObject.Destroy(curModel);
        }

        public override void initialization()
        {
            SkillSystem.skills = new List<Skill>();
            if (curModel != null)
            {
                GameObject.Destroy(curModel);
                curModel = null;
            }
            EriyaModel = Owner.transform;
            //获取背部位置
            ModelInstall = EriyaModel.GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetChild(0);
            //生成模型
            curModel = GameObject.Instantiate(model, ModelInstall);
            if (((Eriya.Eriya)Owner).eriyaMode == Eriya.EriyaMode.Hacking)
            {
                //动画初始化
                ((Eriya.Eriya)Owner).eriyaMode = Eriya.EriyaMode.Bot;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            Owner.BattleSystem.OnBeingDamage += Owner_OnBeingDamage;
        }

        private void Owner_OnBeingDamage(object sender, DamageEventArgs<Bot> e)
        {
            if (Vector3.Dot(e.AttackPosition, new Vector3(Owner.Direction.x, 0, Owner.Direction.z)) < 0 && !e.StrongGPImmuse)
            {
                e.Damage -= 20;
            }
        }
    }
    #endregion
    public class CoreWeaponDusbin : CoreWeapon
    {
        public override void dispose()
        {
            throw new System.NotImplementedException();
        }

        public override void initialization()
        {
        }

        public override void Refresh()
        {
            base.Refresh();
        }
    }
}


