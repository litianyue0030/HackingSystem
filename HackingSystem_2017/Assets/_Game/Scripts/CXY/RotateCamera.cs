using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;
namespace CXY
{
    public class RotateCamera : MonoBehaviour
    {
        float m_rotateX;//鼠标水平移动返回值
        float m_rotateY;//鼠标垂直移动返回值
        public float m_rotateSensitive = 3f;//相机旋转灵敏度
        float m_maxRotateX = 360f;
        float m_minRotateX = -360f;
        float m_maxRotateY = 90f;
        float m_minRotateY = -90f;
        Quaternion m_originRotation;
        // Use this for initialization
        void Start()
        {
            m_originRotation = transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            CameraRotate();
        }
        void CameraRotate()
        {
            m_rotateX += Input.GetAxis("Mouse X") * m_rotateSensitive * Time.timeScale;
            m_rotateY += Input.GetAxis("Mouse Y") * m_rotateSensitive * Time.timeScale;
            m_rotateX = Mathf.Clamp(m_rotateX, m_minRotateX, m_maxRotateX);
            m_rotateY = Mathf.Clamp(m_rotateY, m_minRotateY, m_maxRotateY);
            Quaternion m_quaternionX = Quaternion.AngleAxis(m_rotateX, Vector3.up);
            Quaternion m_quaternionY = Quaternion.AngleAxis(m_rotateY, -Vector3.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_originRotation * m_quaternionX * m_quaternionY, 1);
        }
    }
}