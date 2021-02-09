using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyController : PlayerController_Base
{
    Vector2 m_currentVelocity = new Vector2(1,1);
    Vector2 m_lastInputDirection;
    Vector2 m_lastPositionAfterHittinGround;
    Vector2 m_currentHorizontalVelocity;
    Vector2 m_currentVerticallVelocity;

    float m_currentHorizontalSpeed = 0;
    float m_maxHorizontalSpeed = 10f;

    //Components
    Rigidbody2D m_rb;
    WalkController m_wc;
    CapsuleCollider2D m_capsuleCollider;

    //Jump properties
    float m_minJumpheight = 6f;
    float m_maxJumpScalar = 1.5f;
    float m_timeToReachApex = 1f;
    float m_jumpChargeTime = 2f;
    float m_currentJumpHeight;
    float m_maxJumpHeight;//Will be initialized during Start()

    //Horizontal force properties
    float m_timetoReachMaxSpeedFromInput = 4f;
    //Quick turn

    //Gravity properties
    float m_gravityScalar = 1f;
    float m_timeToReachTerminalVelocity = 1f;
    float m_terminalVelocity = 22f;
    float m_currentVerticalSpeed;

    bool m_grounded = false;
    bool m_bounceless = false;

    //Radar properties
    float m_radarRadius = 5f;

    //Ground Buffer
    float m_groundBufferTime = 0.1f;
    float m_groundBufferHandler;

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
    float m_minSensorLength = 0.75f;
    float m_rbVelocityPercetange = 0.25f;

    //Layers or axis
    const string OBSTACLE = "Obstacle";
    const string THREAT = "Threat";

    //States
    enum BouncyState
    {
        FREE_ROAMING,
        CHAINED_ATTACK
    }

    BouncyState m_CurrentState = BouncyState.FREE_ROAMING;


    int collisionCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_wc = GetComponent<WalkController>();
        m_capsuleCollider = GetComponent<CapsuleCollider2D>();

        m_CurrentState = BouncyState.FREE_ROAMING;

        m_maxJumpHeight = m_minJumpheight * m_maxJumpScalar;
        m_currentJumpHeight = m_minJumpheight;
    }

    // Update is called once per frame
    void Update()
    {
        m_capsuleCollider.isTrigger = true;
        CheckSwapStatus(this, m_wc);
        SetSensors();

        m_currentHorizontalSpeed = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxHorizontalSpeed);
        m_currentVerticalSpeed = Mathf.Clamp(m_currentVerticalSpeed, -m_terminalVelocity, m_terminalVelocity);

        if (m_CurrentState == BouncyState.FREE_ROAMING)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, m_capsuleCollider.size.y / 2 + 0.1f, LayerMask.GetMask("Obstacle"));
            m_grounded = groundHit;
            if (!groundHit) { ApplyGravityIfNotGrounded(); }
            CheckJumpStatus();
            CheckPlayerHorizontalInput();
        }

    }

    private void FixedUpdate()
    {
        transform.position = m_rb.position;

        if (m_CurrentState == BouncyState.FREE_ROAMING)
        {
            m_currentHorizontalVelocity = m_lastInputDirection * m_currentHorizontalSpeed;
            m_currentVerticallVelocity = new Vector2(0, m_currentVerticalSpeed);
            m_currentVelocity = m_currentHorizontalVelocity + m_currentVerticallVelocity;
        }
        else
        {
            m_currentVelocity = m_currentVelocity.normalized * (m_currentHorizontalSpeed + Mathf.Abs(m_currentVerticalSpeed));
        }
           

        m_rb.velocity = m_currentVelocity;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy_Base>())
        {
            Destroy(collision.gameObject);
            CheckEnemyRadar();
        }
        
        if (m_CurrentState == BouncyState.FREE_ROAMING)
        {
            if (m_rb.velocity.y < 0 && m_sensors.DSensor)
            {
                //m_grounded = true;

                m_lastPositionAfterHittinGround = m_rb.position;
            }
            ReactToBorders();
            collisionCounter++;
            //Debug.Log("Collision #: " + collisionCounter);
            //Debug.Log(collision.gameObject.name);
        } 


    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (m_grounded && m_currentVerticalSpeed > 0) { m_grounded = false; }
    }
 //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void ApplyGravityIfNotGrounded()
    {
        //TODO when happy with testing values, move initialization to start, to avoid uneccessary operations
        float gravityAccRate = (m_terminalVelocity / m_timeToReachTerminalVelocity) * m_gravityScalar;
        gravityAccRate *= Time.deltaTime;
        if (!m_grounded && Time.time > m_groundBufferHandler)
        {
            m_currentVerticalSpeed -= gravityAccRate;
            m_bounceless = false;
        }
        else if(m_grounded && Mathf.Abs(m_currentVerticalSpeed) < gravityAccRate)
        {
            m_bounceless = true;
            m_currentVerticalSpeed = 0;

            if (m_bounceless)
            {
                m_rb.position = new Vector2(m_rb.position.x, m_lastPositionAfterHittinGround.y + 0.09f);
            }
            //Reset handler
            m_groundBufferHandler = Time.time + m_groundBufferTime;
        }
    }
 //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void BounceTowardsEnemyInRadar(Transform closestValidEnemy = null)
    {
        if (closestValidEnemy)
        {
            m_CurrentState = BouncyState.CHAINED_ATTACK;

            Vector2 closestEnemyPosition = new Vector2(closestValidEnemy.position.x, closestValidEnemy.position.y);
            m_currentVelocity = (closestEnemyPosition - m_rb.position).normalized;
        }
    }
    private void CheckEnemyRadar()
    {
        //Use a sphere cast to find all enemies within player radar
        Collider2D[] enemiesInRadar = Physics2D.OverlapCircleAll(m_rb.position, m_radarRadius, LayerMask.GetMask(THREAT));
        if(enemiesInRadar.Length == 0)
        {
            m_CurrentState = BouncyState.FREE_ROAMING;
            return;
        }
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
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void CheckJumpStatus()
    {
        if (m_grounded && InputManager.WasJumpPressed())
        {
            //TODO Adjustable jump scalar for when jump is held and then released
            m_currentVerticalSpeed = (2 * m_currentJumpHeight) / m_timeToReachApex;
            m_grounded = false;
            m_currentJumpHeight = m_minJumpheight;
        }
        else if (m_grounded && InputManager.JumpHeld())//Variable jump height
        {
            float jumpAcc = ((m_maxJumpHeight - m_minJumpheight) / m_jumpChargeTime) * Time.deltaTime;
            m_currentJumpHeight += jumpAcc;

            m_currentJumpHeight = Mathf.Clamp(m_currentJumpHeight, m_minJumpheight, m_maxJumpHeight);
        }
    }
 //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void CheckPlayerHorizontalInput()
    {
        Debug.Log(m_currentVelocity);
        if (IsQuickturning())
        {
            float quickTurnRate = (m_maxHorizontalSpeed / m_timetoReachMaxSpeedFromInput ) * Time.deltaTime;
            m_currentHorizontalSpeed -= quickTurnRate;
        }
        else if (InputManager.PressingMovementInput())
        {
            float horizontalAcc = (m_maxHorizontalSpeed / m_timetoReachMaxSpeedFromInput) * Time.deltaTime;
            
            m_currentHorizontalSpeed += horizontalAcc;
            m_lastInputDirection = InputManager.GetMovementInput();
            
        }
       
    }

    private bool IsQuickturning()
    {
        return InputManager.PressingMovementInput() && m_lastInputDirection != InputManager.GetMovementInput() && m_currentHorizontalSpeed > 0;
    }
 //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void ReactToBorders()
    {
        if (!m_bounceless)
        {
            if (m_sensors.USensor || m_sensors.DSensor)
            {
                //Bounce a bit less everytime
                m_currentVerticalSpeed *= -0.75f;
            }
        }
       
        if (m_sensors.RSensor || m_sensors.LSensor)
        {
            m_currentHorizontalSpeed += 2;
            m_lastInputDirection *= -1;
        }
    }
 //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
        sensorLength = Mathf.Clamp(sensorLength, m_minSensorLength, m_minSensorLength + sensorLength);
        m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, sensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));
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

        if (m_rb)
        {
            float sensorLength = m_rb.velocity.magnitude * m_rbVelocityPercetange;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * sensorLength);
            Gizmos.DrawLine(transform.position, transform.position + -transform.up * sensorLength);
            Gizmos.DrawLine(transform.position, transform.position + transform.right * sensorLength);
            Gizmos.DrawLine(transform.position, transform.position + -transform.right * sensorLength);
        }
        

    }
}
