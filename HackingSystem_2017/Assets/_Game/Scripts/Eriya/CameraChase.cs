using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem.Eriya
{
    public class CameraChase : MonoBehaviour
    {
        public GameObject player;
        public Vector3 offset;
        private void Start()
        {
            offset = transform.position - player.transform.position;
        }

        private void Update()
        {
            transform.position = offset + player.transform.position;
        }
    }
}
