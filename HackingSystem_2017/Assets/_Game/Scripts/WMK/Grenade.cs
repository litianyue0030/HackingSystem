using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
namespace HackingSystem.Grenade
{
    public class Grenade : Skill
    {
        public float ExplosionRadious;
        private GameObject bullet;
        //public Transform eye;
        //public Transform bornPlace;
        private Rigidbody rb;
        private Vector3 point;
        //public LineRenderer line;
        private RaycastHit hit;
        public Ray ray;
        private GameObject newBullet = null;
        private float BallSpeed = 0;
        private GameObject Destiny;
        private Vector3 tmp;
        public GameObject player;
        public Vector3 speed;
        public override Animator anim { get; set; }

        //owner
        public SkillSystem owner { get; set; }
        //HitDefend

        //Enter,Execute,Complete,AfterComplete,Exit
        public override void Enter() 
        {
            Physics.gravity = new Vector3(0, -15, 0);
            bullet = (GameObject)Resources.Load("Grenade/bullet");
            //Destiny = Instantiate(bullet);

            player = GameObject.Find("Eriya");
            newBullet =  GameObject.Instantiate(bullet);
        }
        public override void Casting()
        {

        }
        public override void Exit() { }
        public override void Complete() { }
        public override void AfterComplete() { }
        //Decast，Inactive
        public override void Decast() { }
        public override void InActive() { }


        void Update()
        {
            tmp = player.transform.position;
            //Destiny.transform.position = CalDestiny();
            if (hit.collider)
            {

                //line.SetPosition(0, ray.origin);
                //line.SetPosition(1, hit.point);
                
                Debug.DrawLine(player.transform.position, hit.point, Color.red, 5f);

            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("GetKeyDown");
                newBullet = GameObject.Instantiate(bullet);
                //StartCoroutine(StartToHold(newBullet));
                BallSpeed = 0;
            }
            else if (newBullet && Input.GetKey(KeyCode.G))
            {
                Debug.Log("GetKey");
                //newBullet.transform.position = ray.origin + Vector3.Normalize(ray.direction);
                newBullet.transform.position = player.transform.position + new Vector3(ray.direction.x, 0, ray.direction.z).normalized * 1 + new Vector3(0, 0.5f, 0);
                BallSpeed += Time.deltaTime * 10;
            }
            else if (newBullet && Input.GetKeyUp(KeyCode.G))
            {
                Debug.Log("GetKeyUp");
                Debug.Log(ray.direction);
                float ang = Vector3.Angle(ray.direction, new Vector3(0, 1, 0)) / 2;
                //Debug.Log(ang);
                //float distance1 = 0.5f * Mathf.Pow(2, 0.5f) * ( Mathf.Pow(Mathf.Pow(ray.direction.x, 2) * Mathf.Pow(ray.direction.z, 2) , 0.5f) - ray.direction.y);
                //float distance2 = 0.5f * Mathf.Pow(2, 0.5f) * (Mathf.Pow(Mathf.Pow(ray.direction.x, 2) * Mathf.Pow(ray.direction.z, 2), 0.5f) + ray.direction.y);
                Debug.Log(Mathf.Sin(ang * Mathf.PI / 180).ToString() + ' ' + Mathf.Cos(ang * Mathf.PI / 180).ToString());
                speed = newBullet.GetComponent<Rigidbody>().velocity = (new Vector3(ray.direction.x, 0, ray.direction.z).normalized * Mathf.Sin(ang * Mathf.PI / 180) + new Vector3(0, Mathf.Cos(ang * Mathf.PI / 180), 0)) * BallSpeed;

                //newBullet.GetComponent<Rigidbody>().velocity = (ray.direction + new Vector3(0, ray.direction.y, 0)).normalized * 10;
                //Debug.Log(Vector3.Angle(ray.direction, newBullet.GetComponent<Rigidbody>().velocity));
            }

        }

        private void FixedUpdate()
        {

            ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            Debug.Log(ray.direction);
            if (Physics.Raycast(ray, out hit, 50))
            {
                //Debug.Log(hit.collider.gameObject.name+" "+ hit.point);
                //rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                point = hit.point;
            }
        }

        IEnumerator StartToHold(GameObject Bullet)
        {
            yield return new WaitForSeconds(3.0f);
            Collider[] things = Physics.OverlapSphere(Bullet.transform.position, ExplosionRadious);
            Debug.Log(things);
            //此处增加上海判定‘
            GameObject.Destroy(Bullet);
        }
        /// <summary>
        /// 技能发动的条件，在技能发动成功的时候重置
        /// </summary>
        public ExecuteRule RuleCast;
        /// <summary>
        /// 技能完成的条件，在技能释放的时候进行重置
        /// </summary>
        public ExecuteRule RuleComplete;
        /// <summary>
        /// 技能结束的条件，在技能完成的时候重置
        /// </summary>
        public ExecuteRule RuleEnd;


        //
        public SkillActionPhase CurrentPhase;
    }
}

