using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour {

    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;

    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;

    float m_ForwardAmount;
    float m_TurnAmount;

    Rigidbody m_Rigidbody;
    // Use this for initialization
    void Start () {
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void MyMove(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        

        m_ForwardAmount = move.z;
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        //transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        //float angle = Vector3.Angle(transform.forward, m_Cam.forward);
        //transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        transform.forward = -m_Cam.forward;
        m_Rigidbody.velocity = move*10;
    }
    // Update is called once per frame
    void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v * Vector3.forward + h * Vector3.right;
        }
        MyMove(m_Move);
    }

    
}
