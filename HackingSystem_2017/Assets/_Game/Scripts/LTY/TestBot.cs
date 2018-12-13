using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;
using HackingSystem.Eriya;
using HackingSystem.Dustbin;

using WJ;
using WMC;
using LC;

namespace LTY
{
    public class TestBot : Bot
    {
        EriyaMode md = EriyaMode.Hacking;
        public GameObject cam;
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

        public override void HitAnimExecute()
        {
            throw new System.NotImplementedException();
        }
        public bool MovePv
        {
            get; private set;
        }
        public override void MoveFrame()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 move = (v * cam.transform.forward + h * cam.transform.right);
            MovePv = move != Vector3.zero;
            move.y = 0;
            move.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 vel = new Vector3(move.x * 5 * abilities.MoveSpeedRate * abilities.ActionRate, rb.velocity.y, move.z * 5 * abilities.MoveSpeedRate * abilities.ActionRate);
            if (!OnAir && Control.JumpArrowDown())
            {
                vel.y = 15;
            }
            rb.velocity = vel;

        }

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
            //WeaponSystem.BallWeapon = new BallWeapon1();

            eriyaMode = EriyaMode.Hacking;
            //WeaponSystem.MainWeapon = new TestMainWeapon();
            WeaponSystem.SubWeapon = new TestSubWeapon();
            WeaponSystem.CoreWeapon = new TestCoreWeapon();
            //WeaponSystem.BackWeapon = new HackingSystem.Dustbin.BackWeaponDusbinTop();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "MoveBuff")
            {
                BuffSystem.AddBuff(new MoveSpeedBuff(null, 2f, BuffSystem, 3f, BuffType.buff));
                Destroy(other.gameObject);
            }
            if (other.tag == "HeatBuff")
            {
                BuffSystem.AddBuff(new HeatBuff(null, 10f, BuffType.buff, BuffSystem, -2));
                Destroy(other.gameObject);
            }
            if (other.tag == "MPBuff")
            {
                BuffSystem.AddBuff(new MPBuff(null, 10f, BuffType.buff, BuffSystem, 3));
                Destroy(other.gameObject);
            }
            if (other.tag == "FreeFromInjuryBuff")
            {
                BuffSystem.AddBuff(new FreeFromInjuryBuff(null, 3f, BuffSystem));
                Destroy(other.gameObject);
            }
        }
    }

    public class TestMainWeapon : MainWeapon
    {

        public TestMainWeapon() : base()
        {
            List<Skill> binskills = new List<Skill>();

            foreach (var item in binskills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = binskills;
        }

        public override void dispose()
        {
       
        }

        public override void initialization()
        {
           
        }

        public bool LeftPressed = false;
        public override void Refresh()
        {
            base.Refresh();
            if (Owner.Interrupt == 0)
            {
                if (Control.MainWPAttackLDown())
                {
                    SkillSystem.Cast(0);
                    LeftPressed = false;
                }
  
            }
        }
    }

    public class TestSubWeapon : SubWeapon
    {

        public TestSubWeapon() : base()
        {
            List<Skill> binskills = new List<Skill>();
            binskills.Add(new GrapplingHook());

            foreach (var item in binskills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = binskills;
        }

        public override void dispose()
        {

        }

        public override void initialization()
        {

        }

        public bool LeftPressed = false;
        public override void Refresh()
        {
            base.Refresh();
            if (Owner.Interrupt == 0)
            {
                if (Control.SubWPAttackLDown())
                {
                    SkillSystem.Cast(0);
                    LeftPressed = false;
                }
                //if (Control.MainWPAttackRPress())
                //{
                //    SkillSystem.Cast(1);
                //    LeftPressed = false;
                //}
            }
        }
    }

    public class TestCoreWeapon : CoreWeapon
    {
        public TestCoreWeapon() : base()
        {
            List<Skill> binskills = new List<Skill>();
            //binskills.Add(new ThroughWallSkill());
            binskills.Add(new FlashSkill());
            foreach (var item in binskills)
            {
                item.owner = SkillSystem;
            }
            SkillSystem.skills = binskills;
        }

        public override void dispose()
        {

        }

        public override void initialization()
        {

        }

        public bool LeftPressed = false;
        public override void Refresh()
        {
            base.Refresh();
            if (Owner.Interrupt == 0)
            {
                if (Control.CoreArrowDown())
                {
                    SkillSystem.Cast(0);
                    LeftPressed = false;
                }
                //if (Control.MainWPAttackRPress())
                //{
                //    SkillSystem.Cast(1);
                //    LeftPressed = false;
                //}
            }
        }
    }
}
