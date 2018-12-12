using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CXY
{
    public class Heat : MonoBehaviour
    {
        Shader m_originalShader;//对象原始的shader
        Shader m_perspectiveShader;//可被透视的shader

        void Start()
        {
            m_originalShader = this.gameObject.GetComponent<Renderer>().material.shader;
            m_perspectiveShader = Shader.Find("Custom/Perspective");//找到使对象可被透视的shader
        }

        void Update()
        {
            CanBePerspective();
        }
        /// <summary>
        /// 若透视技能被激活，使对象可被透视，否则不能被透视
        /// </summary>
        void CanBePerspective()
        {
            if (Perspective.m_isPerspective)
            {
                this.gameObject.GetComponent<Renderer>().material.shader = m_perspectiveShader;
                
            }
            else
            {
                this.gameObject.GetComponent<Renderer>().material.shader = m_originalShader;
                
            }
        }
    }
}