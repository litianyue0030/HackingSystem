using UnityEngine;
using UnityEditor;

namespace HackingSystem
{
    public class GameSystem
    {
        /// <summary>
        /// 游戏上次暂停的时间
        /// </summary>
        public static float TimePaused { get; set; }
        /// <summary>
        /// 游戏暂停的总时间
        /// </summary>
        public static float TimePausedTotal { get; set; }

        public static bool paused;

        static GameSystem()
        {
        }

        public static void PauseOpen()
        {
            TimePaused = Time.time;
            paused = true;
        }

        public static Bot CurrentPlayer;

        /// <summary>
        /// 将指向某个位置的向量转化为欧拉角（-高低角，平面旋转角）表示
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 ConvertDirationTOEularAngles(Vector3 value)
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

            Matrix4x4 x = m;
            //高低旋转

            Vector3 v = x.rotation.eulerAngles;
            v.x = -Mathf.Asin(value.y) * Mathf.Rad2Deg;

            return v;
        }

        public static void PauseClose()
        {
            TimePaused = Time.time - TimePaused;
            TimePausedTotal += TimePaused;
            paused = false;
        }
    }
}