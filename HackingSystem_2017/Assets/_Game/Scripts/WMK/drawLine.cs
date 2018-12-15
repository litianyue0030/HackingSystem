using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawLine : MonoBehaviour {
    private LineRenderer line;
    public Vector3 speed;
    private Vector3[] point;
    public int n;
    private Transform birthplace;
    Ray ray;
    // Use this for initialization
    void Start () {

        speed = GetComponent<ThrowGranded>().speed;
        line = GetComponent<LineRenderer>();
        point = new Vector3[n];
        line.positionCount = n;
        //birthplace = GetComponent<ThrowGranded>().player.transform;
    }
    
    // Update is called once per frame
    void Update () {
        ray = GetComponent<ThrowGranded>().ray;
        float ang = Vector3.Angle(ray.direction, new Vector3(0, 1, 0)) / 2;
        Debug.Log(ang);
        speed = (new Vector3(ray.direction.x, 0, ray.direction.z).normalized * Mathf.Sin(ang * Mathf.PI / 180) + new Vector3(0, Mathf.Cos(ang * Mathf.PI / 180), 0)) * 10;
        birthplace = GetComponent<ThrowGranded>().player.transform;
        if (line.enabled)
        {
            for (int i = 0; i < n; ++i)
            {
                float v = Mathf.Pow(speed.x * speed.x + speed.z * speed.z, 0.5f);
                float t = speed.y / 15 * 2 / n * i;
                point[i] = birthplace.position + new Vector3(speed.x, 0, speed.z).normalized * v * t + new Vector3(0, speed.y * t - 0.5f * 15 * t * t, 0);
                line.SetPosition(i, point[i]);
            }
        }



    }
    
}
