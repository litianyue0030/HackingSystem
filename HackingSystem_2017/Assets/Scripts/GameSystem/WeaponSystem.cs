using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HackingSystem
{
    public class WeaponSystem
    {
        public WeaponSystem(Bot owner)
        {
            this.owner = owner;
        }

        Bot owner;

        public Bot Owner
        {
            get { return owner; }
        }

        //Ball Weapon
        BallWeapon _BallWeapon;
        public BallWeapon BallWeapon
        {
            get { return _BallWeapon; }
            set
            {
                _BallWeapon = value;
                _BallWeapon.Owner = owner;
            }
        }

        //Main Weapon
        MainWeapon _MainWeapon;
        public MainWeapon MainWeapon
        {
            get { return _MainWeapon; }
            set
            {
                _MainWeapon = value;
                _MainWeapon.Owner = owner;
                value.AnimActive = true;
            }
        }
        //Back Weapon
        BackWeapon _BackWeapon;
        public BackWeapon BackWeapon
        {
            get { return _BackWeapon; }
            set
            {
                _BackWeapon = value;
                _BackWeapon.Owner = owner;
            }
        }
        //Core Weapon
        CoreWeapon _CoreWeapon;
        public CoreWeapon CoreWeapon
        {
            get { return _CoreWeapon; }
            set
            {
                _CoreWeapon = value;
                _CoreWeapon.Owner = owner;
            }
        }

        //Sub Weapon
        SubWeapon _SubWeapon;
        public SubWeapon SubWeapon
        {
            get { return _SubWeapon; }
            set
            {
                _SubWeapon = value;
                _SubWeapon.Owner = owner;
            }
        }

        public void Refresh()
        {
            if (_MainWeapon != null)
            {
                _MainWeapon.Refresh();
            }
            if (_SubWeapon != null)
            {
                _SubWeapon.Refresh();
            }
            if (_BackWeapon != null)
            {
                _BackWeapon.Refresh();
            }
            if (_CoreWeapon != null)
            {
                _CoreWeapon.Refresh();
            }
            if (_BallWeapon != null)
            {
                _BallWeapon.Refresh();
            }
        }

        /// <summary>
        /// 释放中的技能
        /// </summary>
        public Skill skillCasting
        {
            get
            {
                if (_MainWeapon != null && _MainWeapon.SkillSystem.SkillCasting != null)
                {
                    return _MainWeapon.SkillSystem.SkillCasting;
                }
                else if (_SubWeapon != null && _SubWeapon.SkillSystem.SkillCasting != null)
                {
                    return SubWeapon.SkillSystem.SkillCasting;
                }
                else if (_BackWeapon != null && _BackWeapon.SkillSystem.SkillCasting != null)
                {
                    return BackWeapon.SkillSystem.SkillCasting;
                }
                else if (_CoreWeapon != null && _CoreWeapon.SkillSystem.SkillCasting != null)
                {
                    return CoreWeapon.SkillSystem.SkillCasting;
                }
                else if (_BallWeapon != null && _BallWeapon.SkillSystem.SkillCasting != null)
                {
                    return _BallWeapon.SkillSystem.SkillCasting;
                }
                else
                {
                    return null;
                }
            }
        }

        public void DecastSkill()
        {
            if (_MainWeapon != null)
            {
                _MainWeapon.SkillSystem.SkillDecast();
            }
            if (_SubWeapon != null)
            {
                _SubWeapon.SkillSystem.SkillDecast();
            }
            if (_CoreWeapon != null)
            {
                _CoreWeapon.SkillSystem.SkillDecast();
            }
            if (_BackWeapon != null)
            {
                _BackWeapon.SkillSystem.SkillDecast();
            }
            if (_BallWeapon != null)
            {
                _BallWeapon.SkillSystem.SkillDecast();
            }
        }
    }
}
