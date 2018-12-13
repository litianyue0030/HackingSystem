using System.Collections;
using System.Collections.Generic;
using HackingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace LC
{
    public enum GunType
    {
        MachineGun,//冲锋枪
        Shotgun,//散弹枪
        LaserGun,//激光
        SniperRifle//狙击枪
    }
    public class Gun : MonoBehaviour
    {
        public Text text;
        public GunType gunType;
        //创建子弹的预设体
        public GameObject machineGunBullet;
        private GameObject shotGunBullet;
        private GameObject laserGunBullet;
        private GameObject sniperRifleBullet;
        public Transform firePos;//子弹发射口

        private float bulletCount;
        private float bulletCountFull;//弹夹容量
        float timer = -0.05f;//计时器
        bool isFire = true;
        private bool cameraZoom = true;
        public float ZoomLevel;//倍镜大小
        public float ZoomInSpeed = 100f;//放大的速度
        public float ZoomOutSpeed = 100f;//还原的速度
        private float initFOV;//初始化相机的FOV

        private float vignetteAmount = 10.0f;

        void Start()
        {
            initFOV = Camera.main.fieldOfView;
            InitBullet();

        }
        void Update()
        {
            GunAttack();
            text.text = bulletCount.ToString() + "/" + bulletCountFull.ToString();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InitBullet();
            }
        }
        //枪械攻击
        void GunAttack()
        {
            if (Control.MainWPAttackLPress())
            {
                isFire = true;
            }
            else
            {
                isFire = false;
                timer = -0.05f;
            }

            if (isFire)
            {
                switch (gunType)
                {
                    case GunType.LaserGun:
                        LaserGunAttack();
                        break;
                    case GunType.MachineGun:
                        MachineGunAttack();
                        break;
                    case GunType.Shotgun:
                        ShotGunAttack();
                        break;
                    case GunType.SniperRifle:
                        SniperRifleAttack();
                        break;
                }
            }
            //倍镜
            if (Control.MainWPAttackRPress())
            {
                ZoomView();
            }
            else
            {
                ZoomOut();
            }
        }

        void InitBullet()
        {
            switch (gunType)
            {
                case GunType.LaserGun:
                    ZoomLevel = 1;
                    bulletCount = 10;
                    break;
                case GunType.MachineGun:
                    bulletCount = 40;
                    ZoomLevel = 2f;
                    break;
                case GunType.Shotgun:
                    bulletCount = 10;
                    ZoomLevel = 1.5f;
                    break;
                case GunType.SniperRifle:
                    bulletCount = 5;
                    ZoomLevel = 4f;
                    break;
            }

            bulletCountFull = bulletCount;
        }

        /// <summary>
        /// time:枪械的攻击速率,recoilforce: 后坐力
        /// </summary>
        /// <param name="time"></param>
        /// <param name="Recoilforce"></param>
        void shoot(float time, float recoilforce)
        {
            if (bulletCount > 0)
            {
                timer += Time.deltaTime;
                if (timer > time)
                {
                    GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
                    timer = 0;
                    bulletCount--;
                }

                float i = Random.Range(-0.02f, 0.02f);
                firePos.eulerAngles -= new Vector3(recoilforce, i, 0);
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 冲锋枪攻击
        /// </summary>
        void MachineGunAttack()
        {
            shoot(0.1f, 0.07f);
            if (timer > -0.05f && timer < 0)
            {
                bulletCount--;
                GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
                Debug.Log("Fire");
                timer = 0;
            }

        }
        /// <summary>
        /// 散弹枪，后期加入伤害后，根据距离判断伤害，不在发射很多的子弹。
        /// </summary>
        void ShotGunAttack()
        {
            shoot(2f, 0.07f);
            if (timer > -0.05f && timer < 0)
            {
                bulletCount--;
                GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
                timer = 0;
            }
            //Vector3 startEuler = firePos .eulerAngles;
            //float time = 2f;//射击速率
            //timer += Time.deltaTime;
            //if (timer > -0.05f && timer < 0)
            //{

            //    bulletCount--;
            //    for (int i = 0; i < 6; i++)
            //    {
            //        float _Xrotation = Random.Range(-0.02f, 0.02f);
            //        float _Yrotation = Random.Range(-0.02f, 0.02f);
            //        firePos.eulerAngles += new Vector3(_Xrotation, _Yrotation, 0);
            //        GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
            //    }
            //    timer = 0;
            //    float a = Random.Range(-0.02f, 0.02f);
            //    firePos.eulerAngles = startEuler;
            //    firePos.eulerAngles -= new Vector3(0.02f, a, 0);
            //}
            //if (timer   >time)
            //{
            //    bulletCount--;
            //    for (int i = 0; i < 6; i++)
            //    {
            //        float _Xrotation = Random.Range(-0.02f, 0.02f);
            //        float _Yrotation = Random.Range(-0.02f, 0.02f);
            //        firePos.eulerAngles  += new Vector3(_Xrotation , _Yrotation , 0);
            //        GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
            //    }
            //    timer = 0;
            //    float a = Random.Range(-0.02f, 0.02f);
            //    firePos.eulerAngles = startEuler;
            //    firePos.eulerAngles -= new Vector3(0.02f, a, 0);
            //}

        }
        /// <summary>
        /// 激光
        /// </summary>
        void LaserGunAttack()
        {
            shoot(2, 0.07f);
        }
        /// <summary>
        /// 狙击
        /// </summary>
        void SniperRifleAttack()
        {
            shoot(2, 0.07f);
            if (timer > -0.05f && timer < 0)
            {
                GameObjectPool.GetInstance().MachineGunBulletInstantiate(machineGunBullet, firePos);
                timer = 0;
            }

        }
        //倍镜放大
        private void ZoomView()
        {

            if (Mathf.Abs(Camera.main.fieldOfView - (initFOV / ZoomLevel)) < 0.5f)
            {
                Camera.main.fieldOfView -= Time.deltaTime * ZoomInSpeed;
            }
            else if (Camera.main.fieldOfView - (Time.deltaTime * ZoomInSpeed) >= initFOV / ZoomLevel)
            {
                Camera.main.fieldOfView -= Time.deltaTime * ZoomInSpeed;
            }
        }
        //倍镜还原
        private void ZoomOut()
        {
            if (Mathf.Abs(Camera.main.fieldOfView - (initFOV / ZoomLevel)) < 0.5f)
            {
                //Camera.main.fieldOfView = initFOV;
                Camera.main.fieldOfView += Time.deltaTime * ZoomOutSpeed;

            }
            else if (Camera.main.fieldOfView + (Time.deltaTime * ZoomOutSpeed) <= initFOV)
            {
                Camera.main.fieldOfView += Time.deltaTime * ZoomOutSpeed;
            }
        }
    }
}