using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyController : MonoBehaviour
{
    Vector2 m_currentVelocity = new Vector2(1,1);
    float m_acuumulatedSpeed = 1f;
    float m_maxSpeed = 10f;
    Rigidbody2D m_rb;

    //Gravity properties
    float m_gravity;
    float m_terminalVelocity;
    float m_currentVerticalSpeed;

    //Sensor properties
    enum Sensors
    {
        UP,
        UP_RIGHT,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        DOWN_LEFT,
        LEFT,
        LEFT_UP
    }

    struct ActiveSensors
    {
        public RaycastHit2D USensor;
        public RaycastHit2D URSensor;
        public RaycastHit2D RSensor;
        public RaycastHit2D RDSensor;
        public RaycastHit2D DSensor;
        public RaycastHit2D DLSensor;
        public RaycastHit2D LSensor;
        public RaycastHit2D LUSensor;
    }

    ActiveSensors m_sensors;
    float m_sensorLength = 0.75f;

    //Layers or axis
    const string OBSTACLE = "Obstacle";

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SetSensors();
        m_acuumulatedSpeed = Mathf.Clamp(m_acuumulatedSpeed, 0, m_maxSpeed);
    }

    private void FixedUpdate()
    {
        m_rb.velocity = m_currentVelocity * m_acuumulatedSpeed;
        Debug.Log(m_rb.velocity.normalized);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReactToBorders();
    }

    private void ReactToBorders()
    {
        m_acuumulatedSpeed += 2;
        if (m_sensors.USensor || m_sensors.DSensor)
        {
            m_currentVelocity.y *= -1;
        }
        if (m_sensors.RSensor || m_sensors.LSensor)
        {
            m_currentVelocity.x *= -1;
        }
        else
        {
            //m_currentVelocity *= -1;
        }
       
    }

    //private Sensors GetSensorHit()
    //{

    //}
    private void SetSensors()
    {
        m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));

        //m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, m_rb.velocity.magnitude, LayerMask.GetMask(OBSTACLE));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + ((transform.up + transform.right).normalized * m_sensorLength));
        Gizmos.DrawLine(transform.position, transform.position + ((-transform.up + transform.right).normalized * m_sensorLength));
        Gizmos.DrawLine(transform.position, transform.position + ((-transform.up + -transform.right).normalized * m_sensorLength));
        Gizmos.DrawLine(transform.position, transform.position + ((transform.up + -transform.right).normalized * m_sensorLength));
        Gizmos.DrawLine(transform.position, transform.position + transform.up  * m_sensorLength);
        Gizmos.DrawLine(transform.position, transform.position + -transform.up  * m_sensorLength);
        
    }
}
