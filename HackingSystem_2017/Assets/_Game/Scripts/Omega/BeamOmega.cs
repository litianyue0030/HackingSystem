using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem.Omega
{
    public class BeamOmega : MonoBehaviour
    {
        public GameObject MainBeam;
        public GameObject[] SubBeams;
        Vector3 diration = new Vector3(0, 0, 1);
        public Vector3 Diration
        {
            get { return diration; }
            set
            {
                diration =value;
                //Set Rotation
            }
        }
        public float BeamWidth = 4;
        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, 3);
            MainBeam.transform.transform.localScale = MainBeam.transform.localScale * BeamWidth / 4;
            MainBeam.transform.GetChild(2).GetComponent<LineRenderer>().widthMultiplier =  BeamWidth;
            for (int i = 0; i < SubBeams.Length; i++)
            {
                SubBeams[i].transform.transform.localScale = SubBeams[i].transform.localScale * BeamWidth / 4;
                SubBeams[i].transform.GetChild(2).GetComponent<LineRenderer>().widthMultiplier = BeamWidth / 2;
            }
        }
        float StaTime = 0;
        // Update is called once per frame
        void Update()
        {
            float T = Time.time * Mathf.PI*3;
            StaTime += Time.deltaTime;
            for (int i = 0; i < SubBeams.Length; i++)
            {
                SubBeams[i].transform.localPosition = new Vector3(BeamWidth*0.5f * Mathf.Sin(T + i*Mathf.PI / 1.5f), BeamWidth * 0.5f * Mathf.Cos(T + i*Mathf.PI / 1.5f), 0);
                SubBeams[i].transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(0, SubBeams[i].transform.position);
                SubBeams[i].transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(1, SubBeams[i].transform.position + diration * 250);
                if (StaTime > 2.7f)
                {
                    Vector3 sc = MainBeam.transform.localScale;
                    sc.x = sc.y = BeamWidth / 1.2f * (3 - StaTime);
                    SubBeams[i].transform.transform.localScale = 0.5f*sc;
                    SubBeams[i].transform.GetChild(2).GetComponent<LineRenderer>().widthCurve = new AnimationCurve(new Keyframe(0, (3 - StaTime)/0.3f), new Keyframe(1, (3 - StaTime) / 0.3f));
                }
            }
            MainBeam.transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(0, MainBeam.transform.position);
            MainBeam.transform.GetChild(2).GetComponent<LineRenderer>().SetPosition(1, MainBeam.transform.position + diration * 250);
            if (StaTime > 2.7f)
            {
                Debug.Log(0.5f * (3 - StaTime));
                MainBeam.transform.GetChild(2).GetComponent<LineRenderer>().widthCurve = new AnimationCurve(new Keyframe(0, (3 - StaTime) / 0.3f), new Keyframe(1, (3 - StaTime) / 0.3f));
                Vector3 sc = MainBeam.transform.localScale;
                sc.x = sc.y = BeamWidth/ 1.2f * (3 - StaTime);
                MainBeam.transform.transform.localScale = sc;
            }
        }

        
    }
}