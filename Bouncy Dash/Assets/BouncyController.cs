using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyController : MonoBehaviour
{
    Vector2 m_currentDirection = new Vector2(1,1);
    float m_acuumulatedSpeed = 1f;
    float m_maxSpeed = 10f;
    Rigidbody2D m_rb;

    //Gravity properties
    float m_gravity;
    float m_terminalVelocity;
    float m_currentVerticalSpeed;

    //Radar properties
    float m_radarRadius = 5f;

    //Sensor properties
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
    const string THREAT = "Threat";

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
        transform.position = m_rb.position;
        m_rb.velocity = m_currentDirection * m_acuumulatedSpeed;
        Debug.Log(m_rb.velocity.normalized);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy_Base>())
        {
            Destroy(collision.gameObject);  
        }
        ReactToBorders();
        CheckEnemyRadar();
    }
    private void BounceTowardsEnemyInRadar(Transform closestValidEnemy = null)
    {
        if (closestValidEnemy)
        {
            Vector2 closestEnemyPosition = new Vector2(closestValidEnemy.position.x, closestValidEnemy.position.y);
            m_currentDirection = (closestEnemyPosition - m_rb.position).normalized;
        }
    }
    private void CheckEnemyRadar()
    {
        //Use a sphere cast to find all enemies within player radar
        Collider2D[] enemiesInRadar = Physics2D.OverlapCircleAll(m_rb.position, m_radarRadius, LayerMask.GetMask(THREAT));
        HashSet<GameObject> validEnemies = new HashSet<GameObject>();//If there is no obstacle between the player and enemy, then it is a valid path

        //Find all valid enemies in radar
        foreach (Collider2D enemy in enemiesInRadar)
        {
            Vector2 enemyPosition2D = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            Vector2 pathTowardsEnemy = enemyPosition2D - m_rb.position;

            RaycastHit2D hitObstacle = Physics2D.Raycast(m_rb.position, pathTowardsEnemy.normalized, pathTowardsEnemy.magnitude, LayerMask.GetMask(OBSTACLE));
            if (!hitObstacle) { validEnemies.Add(enemy.gameObject); }
        }

        float currentClosestDistance = Mathf.Infinity;
        Transform closestValidEnemy = null;
        //Go through valid enemies if any, to find closest enemy
        if (validEnemies.Count > 0)
        {
            foreach (GameObject enemy in validEnemies)
            {
                Vector2 enemyPosition2D = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                float distanceToEnemy = Vector2.Distance(enemyPosition2D, m_rb.position);

                if (distanceToEnemy < currentClosestDistance)
                {
                    closestValidEnemy = enemy.transform;
                    currentClosestDistance = distanceToEnemy;
                }
            }
        }
       
        //Check if there is a valid path to closest enemy
        if(closestValidEnemy != null) { BounceTowardsEnemyInRadar(closestValidEnemy); }
    }
    private void ReactToBorders()
    {
        m_acuumulatedSpeed += 2;
        if (m_sensors.USensor || m_sensors.DSensor)
        {
            m_currentDirection.y *= -1;
        }
        if (m_sensors.RSensor || m_sensors.LSensor)
        {
            m_currentDirection.x *= -1;
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
