using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem.Eriya
{
    public class Eriya : Bot
    {
        public GameObject[] skill1Bullet;
        public GameObject cam;
        EriyaMode md = EriyaMode.Hacking;

        public EriyaMode eriyaMode
        {
            get { return md; }
            set
            {
                md = value;
                switch (value)
                {
                    case EriyaMode.Hacking:
                        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    case EriyaMode.Bot:
                        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        transform.GetChild(1).gameObject.SetActive(true);
                        break;
                }
            }
        }

        #region 面向信息
        //这里的设置值用角度表示
        Vector2 dirationSet;
        bool settedDiration;
        bool dirationLock = false;
        public override bool DirectionLock
        {
            get
            {
                return dirationLock;
            }

            set
            {
                dirationLock = value;
                if (value)
                {
                    dirationSet = Direction;
                    settedDiration = false;
                }
                else
                {
                    if (settedDiration)
                    {
                        AngleDirection = dirationSet;
                    }
                }
            }
        }

        public override Vector3 Direction
        {
            get
            {
                Vector3 ro = transform.Find("GameObject").eulerAngles * Mathf.Deg2Rad;
                return new Vector3(Mathf.Sin(ro.y) * Mathf.Cos(ro.x), -Mathf.Sin(ro.x), Mathf.Cos(ro.y) * Mathf.Cos(ro.x));
            }

            set
            {
                Vector2 eAngles = GameSystem.ConvertDirationTOEularAngles(value);
                if (dirationLock)
                {
                    dirationSet = eAngles;
                    settedDiration = true;
                    return;
                }
                transform.Find("GameObject").eulerAngles = eAngles;
                transform.Find("Eriya_Skin").eulerAngles = new Vector3(0, eAngles.y, 0);
            }
        }

        public override Vector2 AngleDirection
        {
            get
            {
                return transform.Find("GameObject").eulerAngles;
            }

            set
            {
                if (dirationLock)
                {
                    dirationSet = value;
                    settedDiration = true;
                    return;
                }
                transform.Find("GameObject").eulerAngles = value;
                transform.Find("Eriya_Skin").eulerAngles = new Vector3(0, value.y, 0);
            }
        }

        #endregion

        public override void Initialzation()
        {
            Physics.gravity = new Vector3(0, -30, 0);
            base.Initialzation();
            tag = "Player";
            //Action = new PlayerController();
            Action = new PlayerController();

            abilities = new Abilities(this, 100, 200);
            abilities.KnockBackDefend = 150;

            //MainWeapon = new MainWeaponDusbinTop();
            WeaponSystem.BallWeapon = new BallWeapon1();

            eriyaMode = EriyaMode.Hacking; 
            WeaponSystem.MainWeapon = new Dustbin.MainWeaponDusbinTop();
            WeaponSystem.SubWeapon = new Dustbin.SubWeaponDusbinTop();
            WeaponSystem.BackWeapon = new Dustbin.BackWeaponDusbinTop();
        }
        //上一帧是否有移动
        public bool MovePv
        {
            get; private set;
        }

        public override void MoveFrame(Vector3 MoveMessage)
        {
            //if (Control.UpArrowPress())
            //{
            //    transform.Translate(Vector3.forward * 0.1f, Space.Self);
            //}

            //if (Control.DownArrowPress())
            //{
            //    transform.Translate(Vector3.back * 0.1f, Space.Self);
            //}

            //if (Control.LeftArrowPress())
            //{
            //    transform.Translate(Vector3.left * 0.1f, Space.Self);
            //}

            //if (Control.RightArrowPress())
            //{
            //    transform.Translate(Vector3.right * 0.1f, Space.Self);
            //}
            /*
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            */
            float h = MoveMessage.x;
            float v = MoveMessage.z;

            Vector3 move = (v * cam.transform.forward + h * cam.transform.right);
            MovePv = move != Vector3.zero;
            move.y = 0;
            move.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 vel = new Vector3(move.x * 5 * abilities.MoveSpeedRate * abilities.ActionRate, rb.velocity.y, move.z * 5 * abilities.MoveSpeedRate * abilities.ActionRate);
            if (!OnAir && MoveMessage.y > 0)
            {
                vel.y = 15;
            }
            rb.velocity = vel;

        }

        public override void HitAnimExecute()
        {
            throw new System.NotImplementedException();
        }

        public override bool OnAir
        {
            get
            {
                CapsuleCollider c = GetComponent<CapsuleCollider>();
                Vector3 v = transform.position;
                RaycastHit[] r = Physics.RaycastAll(v, Vector3.down, 0.55f);
                foreach (var item in r)
                {
                    if (item.collider.tag != "Player" && !item.collider.isTrigger)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override Animator Animator
        {
            get
            {
                if (eriyaMode == EriyaMode.Bot)
                {
                    return transform.GetChild(1).GetComponent<Animator>();
                }
                else
                {
                    return transform.GetChild(0).GetChild(0).GetComponent<Animator>();
                }
            }
        }

    }

    public enum EriyaMode
    {
        Hacking,
        Bot
    }

    //public class ActionMove:GameAction<Bot>
    //{
    //    public override void Execute()
    //    {
    //        if (Executor.WeaponSystem.skillCasting == null)
    //        {
    //            Executor.MoveFrame();
    //        }
    //    }
    //}

    public class BallWeapon1 : BallWeapon
    {
        public override void dispose()
        {
        }

        public override void initialization()
        {
            List <Skill>  ballskills = new List <Skill> ();
            ballskills.Add(new SkillBallHack());
            ballskills.Add(new SkillBallLaser());
            for (int i = 0; i < ballskills.Count; i++)
            {
                ballskills[i].owner = SkillSystem;
            }
            SkillSystem.skills = ballskills;
        }

        public override void Refresh()
        {
            if (((Eriya)Owner).eriyaMode == EriyaMode.Bot)
            {
                Debug.Log("Bot");
                return;
            }
            base.Refresh();
            if (Control.MainWPAttackLDown())
            {
                SkillSystem.Cast(0);
            }
            if (Control.MainWPAttackRDown())
            {
                SkillSystem.Cast(1);
            }
            if(Control.BackArrowDown())
            {
                //WJ's GrapplingHook Skill
                SkillSystem.Cast(2);
            }
            if(Control.CoreArrowDown())
            {
                //WMC's Flash Skill
                SkillSystem.Cast(3);
            }
            if(Control.ShiftHackDown())
            {
                //WMC's ThroughWallSkill
                SkillSystem.Cast(4);
            }
        }
    }
    
    class SkillBallLaser : Skill
    {
        LineRenderer LaserLine;
        public SkillBallLaser()
        {
            CurrentPhase = 0;
            //Debug.Log("hh");
            RuleCast = new RuleTimeOver(1.2f);
            RuleComplete = new RuleTrue();
            RuleEnd = new RuleTimeOver(0.3f);
        }

        public override void AfterComplete()
        {
        }


        public override void Casting()
        {
        }

        public override void Complete()
        {
            Bot b = owner.owner.Owner;
            GameObject BulletLaser = GameObject.Instantiate(Resources.Load<GameObject>("Eriya/EriyaLaser"), b.transform.position, b.transform.Find("GameObject").rotation);
            LaserLine = BulletLaser.GetComponent<LineRenderer>();
            LaserLine.SetPosition(0, BulletLaser.transform.Find("Sphere").position);
            RaycastHit[] RC = Physics.RaycastAll(BulletLaser.transform.Find("Sphere").position, b.Direction, 20f);
            Vector3 EndP = BulletLaser.transform.Find("Sphere").position + b.Direction * 20;
            foreach (var rc in RC)
            {
                if (rc.collider.gameObject == owner.owner.Owner.gameObject)
                {
                    continue;
                }
                else
                {
                    EndP = rc.point;
                    //射到玩家身上计算伤害
                    if (rc.collider.gameObject.CompareTag("Player"))
                    {
                        Bot botE = rc.collider.gameObject.GetComponent<Bot>();
                        botE.BattleSystem.BeDamageBySkill(25, this, DamageMode.Energy, 0, b.Direction, rc.point);
                    }
                }
            }
            LaserLine.SetPosition(1, EndP);
            GameObject.Destroy(BulletLaser, 0.1f);
        }

        public override void Decast()
        {
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        
        public override void InActive()
        {
        }
    }
    
    class SkillBallHack : Skill
    {
        GameObject BulletHack;
        GameObject curBulletHack;
        GameObject BulletLaser;
        public Bot HackEnemy = null;
        LineRenderer LaserLine;
        float time;
        public SkillBallHack()
        {
            CurrentPhase = SkillActionPhase.InActive;
            RuleComplete = new RuleTimeOver(2.5f);
            RuleCast = new RuleTimeOver(1.2f);
            RuleEnd = new RuleTrue();
            BulletHack = Resources.Load<GameObject>("Eriya/BulletHack");
            
        }

        public override void AfterComplete()
        {
        }

        public override void Casting()
        {
            time += Time.deltaTime;
            if (time >= 0.1f)
            {
                Bot b = owner.owner.Owner;
                if (curBulletHack != null)
                {
                    GameObject.Destroy(curBulletHack);
                    BulletLaser = GameObject.Instantiate(Resources.Load<GameObject>("Eriya/HackLaser"), b.transform.position, b.transform.Find("GameObject").rotation);
                    LaserLine = BulletLaser.GetComponent<LineRenderer>();
                    curBulletHack = null;
                }//若黑入判定子弹没有销毁销毁子弹
                if (HackEnemy == null)
                {
                    return;
                }//若没有黑入敌人
                if (!Control.MainWPAttackLPress())
                {
                    owner.SkillDecast();
                }//松开左键技能结束
                BulletLaser.transform.position = b.transform.position;
                BulletLaser.transform.rotation = b.transform.Find("GameObject").rotation;
                //玩家和敌人连线
                b.Direction = HackEnemy.transform.position - b.transform.position;
                LaserLine.SetPosition(0, BulletLaser.transform.Find("Sphere").position);
                LaserLine.SetPosition(1, HackEnemy.transform.position);
            }
        }

        public override void Complete()
        {
            if (HackEnemy != null)
            {
                GameObject.Destroy(BulletLaser);
                ((EnemyAndResource)HackEnemy).BeHack();
            }
        }

        public override void Decast()
        {
        }

        public override void Enter()
        {
            HackEnemy = null;
            curBulletHack = GameObject.Instantiate(BulletHack, owner.owner.Owner.transform.position, owner.owner.Owner.transform.Find("GameObject").rotation);
            curBulletHack.GetComponent<BulletHack>().owner = this;
            time = 0;
        }

        public override void Exit()
        {
        }

        public override void InActive()
        {
        }
        
    }

}