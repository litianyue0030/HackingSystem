using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;
using UnityEngine.UI;
namespace CXY
{

    /// <summary>
    /// 透视技能
    /// </summary>
    public class Perspective : Skill
    {
        //CameraWeapon m_cameraWeapon;
        public int m_perspectiveID;
        public static bool m_isPerspective = false;//透视技能是否被激活（默认不激活）

        public override void Enter()
        {

        }
        public override void Casting()
        {

        }
        public override void Exit()
        {

        }
        public override void Complete()
        {

        }
        public override void AfterComplete()
        {

        }
        public override void InActive()
        {
            m_isPerspective = true;
        }
        public override void Decast()
        {
            m_isPerspective = false;
        }
    }
    /// <summary>
    /// 放置摄像头技能
    /// </summary>
    public class PutCamera : Skill
    {

        public PutCamera m_putCamera;
        public int m_putCameraID;
        public int m_cameraCount;
        public override void AfterComplete()
        {

        }

        public override void Casting()
        {

        }

        public override void Complete()
        {

        }

        public override void Decast()
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void InActive()
        {

        }

    }
    public class LaunchFlyer : Skill
    {

        public LaunchFlyer m_launchFlyer;
        public int m_launchFlyerID;
        public override void AfterComplete()
        {

        }

        public override void Casting()
        {

        }

        public override void Complete()
        {

        }

        public override void Decast()
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void InActive()
        {


        }
    }
    //public class RulePers : ExecuteRule
    //{
    //    public override bool RuleExecute()
    //    {
    //        return !Perspective.m_isPerspective;
    //    }
    //}
    /// <summary>
    /// 骇入摄像机获得的武器
    /// </summary>
    //public class CameraWeapon : Weapon
    //{
    //    Perspective m_pers = new Perspective();
    //    PutCamera m_putC = new PutCamera();
    //    LaunchFlyer m_lauF = new LaunchFlyer();
    //    //CameraSkillSystem m_skillsSystem = new CameraSkillSystem();
    //    public List<Skill> m_skills = new List<Skill>();
    //    public bool m_equipCamera;
    //    public override void dispose()
    //    {
    //        m_equipCamera = false;
    //        for (int i = m_skills.Count; i > 0; i--)
    //        {
    //            if (i == m_pers.m_perspectiveID)
    //            {
    //                m_skills.Remove(m_skills[i]);
    //            }
    //            if (i == m_putC.m_putCameraID)
    //            {
    //                m_skills.Remove(m_skills[i]);
    //            }
    //            if (i == m_lauF.m_launchFlyerID)
    //            {
    //                m_skills.Remove(m_skills[i]);
    //            }
    //        }
    //    }

    //    public override void initialization()
    //    {
    //        m_equipCamera = true;
    //        m_skills.Add(m_pers);
    //        m_pers.m_perspectiveID = m_skills.IndexOf(m_pers);
    //        m_skills.Add(m_putC);
    //        m_putC.m_putCameraID = m_skills.IndexOf(m_putC);
    //        m_skills.Add(m_lauF);
    //        m_lauF.m_launchFlyerID = m_skills.IndexOf(m_lauF);

    //    }
    //    public override void Refresh()
    //    {
    //        base.Refresh();
    //    }
    /// <summary>
    /// 满足条件开启透视技能
    /// </summary>
    //public void PerspectiveSkill()
    //{
    //    if (m_skills[m_pers.m_perspectiveID] != null)
    //    {
    //        m_skills[m_pers.m_perspectiveID].Enter();
    //        m_skills[m_pers.m_perspectiveID].Casting();
    //        m_skills[m_pers.m_perspectiveID].Exit();
    //        m_skills[m_pers.m_perspectiveID].Complete();
    //        m_skills[m_pers.m_perspectiveID].AfterComplete();
    //        if (Control.CoreArrowDown() && !Perspective.m_isPerspective)
    //        {
    //            m_skills[m_pers.m_perspectiveID].InActive();
    //            return;
    //        }
    //        if (Control.CoreArrowDown() && Perspective.m_isPerspective)
    //        {
    //            m_skills[m_pers.m_perspectiveID].Decast();
    //            return;
    //        }
    //    }
    //}

    public class CameraSkills : Bot
    {

        public bool m_equipCameraWeapon = true;
        public bool m_dispose = false;
        //CameraWeapon m_camera = new CameraWeapon();
        //Perspective m_perspective = new Perspective();
        public override bool DirectionLock { get; set; }
        public override Vector3 Direction { get; set; }
        public override Vector2 AngleDirection { get; set; }
        public float m_time = 5f;//飞行器存在时间
        public LayerMask m_mask;
        bool m_flyerOn;//是否已存在无人机
        Object m_flyPreb;//飞行器预制体
        Object m_cameraPreb;//摄像头预制体
        GameObject m_flyerObject;//飞行器对象
        GameObject m_cameraObject;//摄像头对象
        GameObject m_flyerWindowObject;//飞行器显示窗口对象
        GameObject m_cameraWindow;//摄像头显示窗口对象
        GameObject m_player;//玩家对象
        int m_timerCount = 0;//正在执行的计时器个数
        int m_maxRotateCameraCount = 1;//摄像头最大个数
        int m_RotateCameraCount;//放置摄像头个数
        RaycastHit m_mouseRayHit;
        Ray m_mouseRay;

        public override bool OnAir
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        //public override RuntimeAnimatorController CurrentAnimatorController { get; set; }

        public override Animator Animator
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override void HitAnimExecute()
        {

        }
        public override void MoveFrame()
        {

        }
        public override void Initialzation()
        {
            base.Initialzation();
            GetObject();

        }
        public override void Refresh()
        {
            //base.Refresh();

            LoadPrefabs();
            if (m_equipCameraWeapon)
            {
                m_equipCameraWeapon = false;
                //m_camera.initialization();
            }
            PerspectiveSkill();
            LaunchFlyerSkill();
            PutRotateCamera();
            //if (m_dispose)
            //{
            //    m_camera.dispose();
            //}
        }
        /// <summary>
        /// 获相关对象并进行初始化
        /// </summary>
        void GetObject()
        {
            m_flyerWindowObject = GameObject.FindGameObjectWithTag("FlyerWindow");
            m_flyerWindowObject.GetComponent<RawImage>().enabled = false;
            m_player = GameObject.FindGameObjectWithTag("Player");
            m_cameraWindow = GameObject.FindGameObjectWithTag("CameraWindow");
            m_cameraWindow.GetComponent<RawImage>().enabled = false;
        }
        /// <summary>
        /// 加载Resourse下的预制体
        /// </summary>
        public void LoadPrefabs()
        {
            m_flyPreb = Resources.Load("CXY/Prefabs/FlyerCamera");
            m_cameraPreb = Resources.Load("CXY/Prefabs/RotateCamera");
        }
        /// <summary>
        /// 透视技能（E键）
        /// </summary>
        public void PerspectiveSkill()
        {

            if (Control.CoreArrowDown() && !Perspective.m_isPerspective)
            {
                Perspective.m_isPerspective = true;
                return;
            }
            if (Control.CoreArrowDown() && Perspective.m_isPerspective)
            {
                Perspective.m_isPerspective = false;
                return;
            }
        }
        /// <summary>
        /// 发射无人机，并在一段时间后或再次按下启动键后收回无人机（Q键）
        /// </summary>
        public void LaunchFlyerSkill()
        {
            if (Control.BackArrowDown() && !m_flyerOn)
            {
                m_flyerWindowObject.GetComponent<RawImage>().enabled = true;
                m_flyerObject = Instantiate(m_flyPreb, m_player.transform.position + new Vector3(0, 3, 0), Quaternion.Euler(90, 0, 0)) as GameObject;
                m_flyerOn = true;
                StartCoroutine(Timer());
                return;
            }
            if (Control.BackArrowDown() && m_flyerOn)
            {
                m_flyerWindowObject.GetComponent<RawImage>().enabled = false;
                m_flyerOn = false;
                Destroy(m_flyerObject);
                return;
            }
        }
        /// <summary>
        /// 放置摄像头（鼠标左键）
        /// </summary>
        public void PutRotateCamera()
        {
            if (Input.GetMouseButtonDown(0) && m_RotateCameraCount < m_maxRotateCameraCount)
            {
                m_mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(m_mouseRay, out m_mouseRayHit, m_mask))
                {
                    Debug.Log(m_mouseRayHit.point);
                    //Vector3 m_hitNormal = m_mouseRayHit.normal;
                    m_cameraWindow.GetComponent<RawImage>().enabled = true;
                    m_cameraObject = Instantiate(m_cameraPreb, m_mouseRayHit.point, m_player.transform.rotation) as GameObject;
                    m_RotateCameraCount++;
                }
            }
            if (Input.GetMouseButtonDown(1) && m_RotateCameraCount != 0)
            {
                m_cameraWindow.GetComponent<RawImage>().enabled = false;
                Destroy(m_cameraObject);
                m_RotateCameraCount--;
            }
        }
        IEnumerator Timer()
        {
            m_timerCount++;
            if (Control.BackArrowDown() && m_flyerOn)
            {

                yield return new WaitForSeconds(m_time);
                if (m_flyerOn && m_timerCount == 1)
                {
                    m_flyerWindowObject.GetComponent<RawImage>().enabled = false;
                    Destroy(m_flyerObject);
                }
                m_timerCount--;
            }
        }
    }
}



