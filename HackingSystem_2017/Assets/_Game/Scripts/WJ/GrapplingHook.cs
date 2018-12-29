using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HackingSystem;

namespace WJ {
    public class GrapplingHook : Skill
    {
        
        /*
         * 向鼠标指定方向射出一个长度为X的钩锁，勾中目标后，X秒内将自己拉向目标。目标仅为建筑物。
         * 
         * Enter()中不确定属于哪个武器
         * 缺少动画
         */

        //移动总时长
        float transDur = 1f;
        //钩爪发射器初始位置，相对人物
        Vector3 instPos = new Vector3(0.10f, 0.06f, 0.10f);
        //绳子初始比例
        float origScale = 0.03f;
        //发射总时长
        float ropePreTime = 0.5f;
        //传送结束的判定距离
        float endDist = 0.25f;

        Vector3 targetPos = Vector3.zero;
        bool isPreparingTrans = false;
        float ropeExtendSpeed = 0f;
        float ropeTotalLength = 0f;
        float transSpeed = 0f;

        bool isTrans = false;
        public GameObject grapplingShooterPrefab;
        GameObject grapplingShooter;

        public GrapplingHook()
        {
            //技能状态
            CurrentPhase = SkillActionPhase.InActive;
            //技能发动的条件，在技能发动成功的时候重置
            RuleCast = new RuleTimeOver(ropePreTime);
            //技能完成的条件，在技能释放的时候进行重置
            RuleComplete = new RuleTimeOver(ropePreTime);
            //技能结束的条件，在技能完成的时候重置
            RuleEnd = new RuleTimeOver(transDur);
            grapplingShooterPrefab = Resources.Load<GameObject>("WJ/Grappling Shooter");
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        ShootHook();
        //    }
        //    if (isPreparingTrans)
        //    {
        //        AdjustRope();
        //    }
        //    if (isTrans)
        //    {
        //        TransCasting();
        //        AdjustRope();
        //    }
        //}

        public override void Enter()
        {
            ///
            ///还不确定属于哪个武器
            ///
            //owner.owner.Owner.ActiveWeapon = WeaponType.MainWeapon;

            ///
            ///动画相关
            ///
            //RuleEnd = new RuleAnimatorPhaseIn();

            ShootHook();
        }

        public override void Decast()
        {
            /* Empty */
        }

        public override void Casting()
        {
            /* EMpty */
            if (isPreparingTrans)
            {
                AdjustRope();
            }
        }

        private void ShootHook()
        {
            RaycastHit hit;
            Transform mainCamera = Camera.main.transform;
            if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100f))
            {
                targetPos = hit.point;

                float dist = Vector3.Distance(owner.owner.Owner.transform.position, targetPos);
                if (dist < 20f)
                {
                    PrepareTrans();
                }
            }
        }

        private void AdjustRope()
        {
            if (!grapplingShooter)
            {
                return;
            }

            Quaternion grapplingShooterRot = Quaternion.LookRotation(targetPos - grapplingShooter.transform.position);
            grapplingShooter.transform.rotation = grapplingShooterRot;
            ropeTotalLength = Vector3.Distance(targetPos, grapplingShooter.transform.position);
            if (isPreparingTrans)
            {
                //shooting rope
                if (grapplingShooter.transform.localScale.z < ropeTotalLength / 2)
                {
                    grapplingShooter.transform.localScale = new Vector3(
                        grapplingShooter.transform.localScale.x,
                        grapplingShooter.transform.localScale.y,
                        grapplingShooter.transform.localScale.z + ropeExtendSpeed * Time.deltaTime);
                }
                //shooting complete
                else
                {
                    isPreparingTrans = false;
                    transSpeed = ropeTotalLength / transDur;
                    isTrans = true;
                }
            }
            else if (isTrans)
            {
                float curDist = Vector3.Distance(targetPos, grapplingShooter.transform.position);
                grapplingShooter.transform.localScale = new Vector3(
                    grapplingShooter.transform.localScale.x,
                    grapplingShooter.transform.localScale.y,
                    curDist / 2);
            }
        }


        private void PrepareTrans()
        {
            isPreparingTrans = true;
            ///
            ///
            ///
            //m_rigidbody.useGravity = false;
            Quaternion grapplingShooterRot = Quaternion.LookRotation(targetPos - owner.owner.Owner.transform.position);
            grapplingShooter = GameObject.Instantiate(grapplingShooterPrefab, owner.owner.Owner.transform.position + instPos, grapplingShooterRot, owner.owner.Owner.transform);
            grapplingShooter.transform.localScale = 0.03f * Vector3.one;

            ropeTotalLength = Vector3.Distance(targetPos, grapplingShooter.transform.position);
            ropeExtendSpeed = ropeTotalLength / ropePreTime;
        }

        private void TransCasting()
        {
            owner.owner.Owner.transform.position = Vector3.MoveTowards(owner.owner.Owner.transform.position, targetPos, transSpeed * Time.deltaTime);
            //disable move controller
            //disable camera move
            if (Vector3.Distance(owner.owner.Owner.transform.position, targetPos) < endDist)
            {
                Complete();
            }
        }


        public override void Exit()
        {
            /* Empty */
            targetPos = Vector3.zero;
            transSpeed = 0f;
            isTrans = false;
            ///
            ///
            ///
            //m_rigidbody.useGravity = true;
            GameObject.Destroy(grapplingShooter);
            ropeTotalLength = 0f;
            ropeExtendSpeed = 0f;
        }

        public override void Complete()
        {
            //targetPos = Vector3.zero;
            //transSpeed = 0f;
            //isTrans = false;
            /////
            /////
            /////
            ////GetComponent<Rigidbody>().useGravity = true;
            //GameObject.Destroy(grapplingShooter);
            //ropeTotalLength = 0f;
            //ropeExtendSpeed = 0f;
        }

        public override void AfterComplete()
        {
            /* Empty */
            if (isTrans)
            {
                TransCasting();
                AdjustRope();
            }
        }

        public override void InActive()
        {
            /*Empty */
        }

    }
}


//public class GrapplingHook : MonoBehaviour {

//    /*
//    * 向鼠标指定方向射出一个长度为X的钩锁，勾中目标后，X秒内将自己拉向目标。目标仅为建筑物。
//    */

//    //移动总时长
//    float transDur = 1f;
//    //钩爪发射器初始位置，相对人物
//    Vector3 instPos = new Vector3(0.10f, 0.06f, 0.10f);
//    //绳子初始比例
//    float origScale = 0.03f;
//    //发射总时长
//    float ropePreTime = 0.5f;
//    //传送结束的判定距离
//    float endDist = 0.25f;

//    Vector3 targetPos = Vector3.zero;
//    float transSpeed = 0f;
//    bool isPreparingTrans = false;
//    float ropeExtendSpeed = 0f;
//    float ropeTotalLength = 0f;

//    bool isTrans = false;
//    public GameObject grapplingShooterPrefab;
//    GameObject grapplingShooter;

//    private void Update() {
//        if (Input.GetKeyDown(KeyCode.E) && !isPreparingTrans && !isTrans) {
//            ShootHook();
//        }
//        if (isPreparingTrans) {
//            AdjustRope();
//        }
//        if (isTrans) {
//            TransCasting();
//            AdjustRope();
//        }
//    }

//    /// <summary>
//    /// 发射钩爪
//    /// 目标点由屏幕中心发出的射线确定，如果距离小于20，发射成功
//    /// </summary>
//    private void ShootHook() {
//        RaycastHit hit;
//        Transform mainCamera = Camera.main.transform;
//        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100f)) {
//            targetPos = hit.point;

//            float dist = Vector3.Distance(transform.position, targetPos);
//            if (dist < 20f) {
//                PrepareTrans();
//            }
//        }
//    }

//    /// <summary>
//    /// 调整绳子状态
//    /// 先调整方向，计算距离
//    /// 如果是准备状态，以一定速度伸长绳子；绳子到位之后，切换状态，按照绳子长度设置速度；施放状态同样按照绳子长度设置速度
//    /// </summary>
//    private void AdjustRope() {
//        if (!grapplingShooter) {
//            return;
//        }

//        Quaternion grapplingShooterRot = Quaternion.LookRotation(targetPos - grapplingShooter.transform.position);
//        grapplingShooter.transform.rotation = grapplingShooterRot;
//        ropeTotalLength = Vector3.Distance(targetPos, grapplingShooter.transform.position);
//        if (isPreparingTrans) {
//            //shooting rope
//            if (grapplingShooter.transform.localScale.z < ropeTotalLength / 2) {
//                grapplingShooter.transform.localScale = new Vector3(
//                    grapplingShooter.transform.localScale.x,
//                    grapplingShooter.transform.localScale.y,
//                    grapplingShooter.transform.localScale.z + ropeExtendSpeed * Time.deltaTime);
//            }
//            //shooting complete
//            else {
//                isPreparingTrans = false;
//                transSpeed = ropeTotalLength / transDur;
//                isTrans = true;
//            }
//        } else if (isTrans) {
//            float curDist = Vector3.Distance(targetPos, grapplingShooter.transform.position);
//            grapplingShooter.transform.localScale = new Vector3(
//                grapplingShooter.transform.localScale.x,
//                grapplingShooter.transform.localScale.y,
//                curDist / 2);
//        }
//    }

//    /// <summary>
//    /// 准备状态
//    /// 人与目标点没有相对位移的情况下，在0.5内将钩爪（绳子）发射到目标点
//    /// </summary>
//    private void PrepareTrans() {
//        isPreparingTrans = true;
//        GetComponent<Rigidbody>().useGravity = false;
//        Quaternion grapplingShooterRot = Quaternion.LookRotation(targetPos - transform.position);
//        grapplingShooter = Instantiate(grapplingShooterPrefab, transform.position + instPos, grapplingShooterRot, transform);
//        grapplingShooter.transform.localScale = 0.03f * Vector3.one;

//        ropeTotalLength = Vector3.Distance(targetPos, grapplingShooter.transform.position);
//        ropeExtendSpeed = ropeTotalLength / ropePreTime;
//    }

//    /// <summary>
//    /// 施放状态
//    /// 朝目标点移动
//    /// 
//    /// 还需要禁用位移，禁用相机运动
//    /// </summary>
//    private void TransCasting() {
//        transform.position = Vector3.MoveTowards(transform.position, targetPos, transSpeed * Time.deltaTime);
//        //disable move controller
//        //disable camera move
//        if (Vector3.Distance(transform.position, targetPos) < endDist) {
//            TransComplete();
//        }
//    }
    
//    /// <summary>
//    /// 施放完成
//    /// 重置所有状态
//    /// </summary>
//    private void TransComplete() {
//        targetPos = Vector3.zero;
//        transSpeed = 0f;
//        isTrans = false;
//        GetComponent<Rigidbody>().useGravity = true;
//        Destroy(grapplingShooter);
//        ropeTotalLength = 0f;
//        ropeExtendSpeed = 0f;
//    }
//}