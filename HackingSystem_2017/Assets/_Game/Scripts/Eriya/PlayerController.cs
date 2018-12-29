using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{

    public class PlayerController : GameAction<Bot>
    {
        private Transform m_Transform;
        //相机视角
        //方向灵敏度  
        public float sensitivityX = 10F;
        public float sensitivityY = 10F;

        //上下最大视角(Y视角,斜率计算)  
        public float minimumY = -0.8f;
        public float maximumY = 0.8f;

        float rotationY = 0F;
        float rotationX = 0F;

        internal override Bot Executor
        {
            get
            {
                return base.Executor;
            }

            set
            {
                base.Executor = value;
                m_Transform = Executor.transform.Find("CameraGroup");
            }
        }

        public override void Enter()
        {
        }

        public override void Execute()
        {
            Vector3 di = m_Transform.eulerAngles;
            /*
            Vector3 diXZ = di;
            diXZ.y = 0;
            diXZ.Normalize();
            */
            float Rotate = sensitivityX * Time.deltaTime*Input.GetAxis("Mouse X")*30;
            
            //Vector3 R = Vector3.Cross(Vector3.up, di);//获取和y轴以及面向垂直的水平向量

            float RotateY = -sensitivityY * Time.deltaTime * Input.GetAxis("Mouse Y")*30;
            //Vector3 RY = Vector3.Cross(di, R).normalized;//获取和当前面向平面垂直的竖直向量
            //Debug.Log((R * Rotate + RY * RotateY));

            //di += (R*Rotate + RY*RotateY);

            di.x += RotateY;
            di.y += Rotate;
            di.x = (di.x + 180) % 360 - 180;
            //Debug.Log(di);
            if (di.x>70f)
            {
                di.x = 70f;
            }
            else if (di.x<-70f)
            {
                di.x = -70f;
            }
            Executor.AngleDirection = di;
            m_Transform.eulerAngles = di;
            /*
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, m_Transform.rotation, Vector3.one);
            
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            Debug.Log(Input.GetAxis("Mouse X"));
            Debug.Log(m_Transform.localEulerAngles.y);
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            //角度限制. rotationY小于min,返回min. 大于max,返回max. 否则返回value   
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            //总体设置一下相机角度  

            Vector3 ro = new Vector3(-rotationY, rotationX, 0) * Mathf.Deg2Rad;
            Executor.Diration = new Vector3(Mathf.Sin(ro.y) * Mathf.Cos(ro.x), -Mathf.Sin(ro.x), Mathf.Cos(ro.y) * Mathf.Cos(ro.x));
            */
            if (Executor.WeaponSystem.skillCasting == null)
            {
                Vector2 move = Control.GetMove();
                Executor.MoveFrame(new Vector3(move.x, Control.JumpArrowDown() ? 1 : 0, move.y));
            }
            
            //this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))*Time.deltaTime);
        }
    }
}