using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    private float xVelocity = 0;

    [SerializeField]
    private float m_xAcceleration;
    [SerializeField]
    private float m_maxSpeed;
    [SerializeField]
    private float m_minJumpHeight;
    [SerializeField]
    private float m_maxJumpHeight;
    [SerializeField]
    private float m_deadZone;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) <= m_deadZone)
        {
            xVelocity = 0;
        }
        else
        {
            xVelocity = Mathf.Clamp(m_rigidBody.velocity.x + Input.GetAxis("Horizontal"), -m_maxSpeed, m_maxSpeed);
        }
        m_rigidBody.velocity = Vector3.right * xVelocity;
    }
}