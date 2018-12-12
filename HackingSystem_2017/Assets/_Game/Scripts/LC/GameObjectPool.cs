using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LC
{
    public class GameObjectPool : MonoBehaviour
    {
        public Transform parent;
        //集合里面的元素，相当于对象池里面的对象，这里的集合可看作为对象池。
        private List<GameObject> pools = new List<GameObject>();
        //首先创建一个单例
        private GameObjectPool()
        {
        }

        private static GameObjectPool instance;

        public static GameObjectPool GetInstance()
        {
            if (instance == null)
            {
                //动态的生成一个名为“GameObjectPool”的对象并将单例脚本附加上去
                instance = new GameObject("GameObjectPool").AddComponent<GameObjectPool>();
            }
            return instance;
        }

        //从对象池中取对象
        public GameObject MachineGunBulletInstantiate(GameObject Bullet, Transform firePos)
        {
            parent = firePos;
            //如果对象池中没有可以对象
            if (pools.Count == 0)
            {
                //就实例化一个新的对象
                GameObject go = Instantiate(Bullet, firePos.position, firePos.rotation) as GameObject;
                go.transform.parent = firePos;
                return go;
            }
            else
            {
                //取出对象池里面的第一个元素
                GameObject obj = pools[0];
                //将对象设置为激活状态
                obj.SetActive(true);
                //将被取出的元素，从对象池中移除
                pools.Remove(obj);
                return obj;
            }
        }


        //向对象池里面存对象
        public void MyDisable(GameObject name)
        {
            //将传进来的对象隐藏（处于非激活状态）
            name.SetActive(false);
            //添加到对象池中（添加到集合中）
            pools.Add(name);
        }
    }
}