using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private bool grounded = false;
    private bool jumping = false;

    [SerializeField]
    private float m_walkAcceleration;
    [SerializeField]
    private float m_maxSpeed;
    [SerializeField]
    private float m_minJumpHeight;
    [SerializeField]
    private float m_maxJumpHeight;
    [SerializeField]
    private float m_gravity;
    [SerializeField]
    private float m_terminalVelocity;
    [SerializeField]
    private float m_deadZone;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, -transform.up, 0.415f) && !jumping)
        {
            grounded = true;
            velocity.y = 0;
        }
        else
        {
            grounded = false;
            velocity.y = Mathf.Clamp(velocity.y - m_gravity, -m_terminalVelocity, m_terminalVelocity);

        }
        if (Mathf.Abs(Input.GetAxis("Horizontal")) <= m_deadZone)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Horizontal") * m_walkAcceleration, -m_maxSpeed, m_maxSpeed);
        }
        transform.position += velocity * Time.deltaTime;
    }
}