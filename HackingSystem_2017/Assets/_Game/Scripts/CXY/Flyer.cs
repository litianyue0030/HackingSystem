using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CXY {
    public class Flyer : MonoBehaviour {
        Vector3 m_pos;
        float m_hight=20f;//飞行器高度
        GameObject m_player;
        RaycastHit m_hit;
        public LayerMask m_mask;
        // Use this for initialization
        void Start() {
            m_player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update() {
            Physics.Raycast(m_player.transform.position + new Vector3(0, 100, 0), Vector3.down, out m_hit, 200f, m_mask);
            m_pos.x = m_player.transform.position.x;
            m_pos.z = m_player.transform.position.z;
            m_pos.y = m_hit.point.y + m_hight;
            Vector3 m_currentPos = transform.position;
            transform.position = Vector3.Lerp(m_currentPos, m_pos,0.1f);
        }
    }
}
