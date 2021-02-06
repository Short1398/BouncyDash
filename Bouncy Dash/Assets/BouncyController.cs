using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyController : MonoBehaviour
{
    Vector2 m_currentVelocity = new Vector2(1,1);
    float m_currentHorizontalSpeed = 0;
    float m_maxHorizontalSpeed = 10f;
    Rigidbody2D m_rb;

    //Gravity properties
    float m_gravityScalar = 1f;
    float m_timeToReachTerminalVelocity = 1f;
    float m_terminalVelocity = 22f;
    float m_currentVerticalSpeed;

    bool m_grounded = false;

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
    float m_rbVelocityPercetange = 0.15f;

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

        m_currentHorizontalSpeed = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxHorizontalSpeed);
        m_currentVerticalSpeed = Mathf.Clamp(m_currentVerticalSpeed, -m_terminalVelocity, m_terminalVelocity);
 
        ApplyGravityIfNotGrounded();
    }

    private void FixedUpdate()
    {
        transform.position = m_rb.position;

        Vector2 horizontalVelocity = new Vector2(m_currentHorizontalSpeed, 0);
        Vector2 verticalVelocity = new Vector2(0, m_currentVerticalSpeed);
        m_currentVelocity = horizontalVelocity + verticalVelocity;

        m_rb.velocity = m_currentVelocity;

        Debug.Log(m_currentVelocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy_Base>())
        {
            Destroy(collision.gameObject);  
        }
        if (m_rb.velocity.y < 0 && m_sensors.DSensor)
        {
            m_grounded = true;
        }
        ReactToBorders();
        CheckEnemyRadar();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_grounded) { m_grounded = false; }
    }

    private void ApplyGravityIfNotGrounded()
    {
        //TODO when happy with testing values, move initialization to start, to avoid uneccessary operations
        float gravityAccRate = (m_terminalVelocity / m_timeToReachTerminalVelocity) * m_gravityScalar;
        gravityAccRate *= Time.deltaTime;
        if (!m_grounded)
        {
            m_currentVerticalSpeed -= gravityAccRate;
        }
    }
    private void BounceTowardsEnemyInRadar(Transform closestValidEnemy = null)
    {
        if (closestValidEnemy)
        {
            Vector2 closestEnemyPosition = new Vector2(closestValidEnemy.position.x, closestValidEnemy.position.y);
            m_currentVelocity = (closestEnemyPosition - m_rb.position).normalized;
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
        
        if (m_sensors.USensor || m_sensors.DSensor)
        {
            //Bounce a bit less everytime
            m_currentVerticalSpeed *= -0.75f;
        }
        if (m_sensors.RSensor || m_sensors.LSensor)
        {
           m_currentHorizontalSpeed = (m_currentHorizontalSpeed + 2) * -1;
        }
    }

    //private Sensors GetSensorHit()
    //{

    //}
    private void SetSensors()
    {
        //m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        float sensorLength = m_rb.velocity.magnitude * m_rbVelocityPercetange;
        m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized,sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up,sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized,sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right,sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, sensorLength, LayerMask.GetMask(OBSTACLE));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float sensorLength = m_rb.velocity.magnitude * m_rbVelocityPercetange;
        Gizmos.DrawLine(transform.position, transform.position + transform.up  *sensorLength);
        Gizmos.DrawLine(transform.position, transform.position + -transform.up  * sensorLength);
        Gizmos.DrawLine(transform.position, transform.position + transform.right  *sensorLength);
        Gizmos.DrawLine(transform.position, transform.position + -transform.right  *sensorLength);
        
    }
}
