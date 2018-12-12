using System;
using System.Collections;
using System.Collections.Generic;
using HackingSystem;
using UnityEngine;

namespace LC
{
    enum BulletType
    {
        machineGunBullet,
        shotGunBullet,
        laserGunBullet,
        sniperRifleBullet
    }

    public class Bullet : MonoBehaviour
    {
        private BulletType bulletType;
        private float bulletDamage;
        private float time;//根据时间判定射击距离  S=vt;
                           //初始化子弹伤害与存在时间
        void InitBullet()
        {
            switch (bulletType)
            {
                case BulletType.laserGunBullet:
                    bulletDamage = 10;
                    time = 1;
                    break;
                case BulletType.machineGunBullet:
                    bulletDamage = 20;
                    time = 1;
                    break;
                case BulletType.shotGunBullet:
                    bulletDamage = 30;
                    time = 1;
                    break;
                case BulletType.sniperRifleBullet:
                    bulletDamage = 40;
                    time = 1;
                    break;
            }
        }

        void OnEnable()
        {
            InitBullet();
            //脚本可用的时候，重置子弹的位置。
            //如果不加这句代码，从对象池中取出的子弹就会从上一次消失的位置开始运动。而不是你设定的子弹生成位置
            transform.position = GameObjectPool.GetInstance().parent.position;
            transform.rotation = GameObjectPool.GetInstance().parent.rotation;
            //开启协程方法
            StartCoroutine(DelayDisable(3f));
        }
        void Update()
        {
            //子弹生成，自动向前运动
            transform.Translate(Vector3.forward * Time.deltaTime * 20);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
            {
                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponent<Bot>().Abilities.HP.Value -= (int)bulletDamage;
                }
            }
        }
        void OnDisable()
        {
            Debug.Log("I'm over");
        }
        //子弹消失的方法
        IEnumerator DelayDisable(float time)
        {
            //等待三秒
            yield return new WaitForSeconds(time);
            //调用单例中向对象池里面存对象的方法
            GameObjectPool.GetInstance().MyDisable(gameObject);
        }

    }
}