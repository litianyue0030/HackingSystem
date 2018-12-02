using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public GameObject player;
    //public GameObject cam;
    Vector3 offset;
	// Use this for initialization
	private void Start () {
        offset = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position =player.transform.position+ offset;
    }
}
