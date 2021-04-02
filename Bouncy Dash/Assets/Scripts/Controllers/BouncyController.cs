using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class BouncyController : PlayerController_Base
{ 
    //Vector2 m_currentVelocity = new Vector2(1, 1);
    //Vector2 m_lastInputDirection;
    //Vector2 m_lastPositionAfterHittinGround = Vector2.zero;
    //Vector2 m_currentHorizontalVelocity;
    //Vector2 m_currentVerticallVelocity;

//Vector2 m_StunnedDirection;

//float m_currentHorizontalSpeed = 0;
//[SerializeField] float m_maxHorizontalSpeed = 10f;

////Components
//Rigidbody2D m_rb;
//WalkController m_wc;
//CapsuleCollider2D m_capsuleCollider;

////Jump properties
//[Header("Jump")]
//[SerializeField] float m_minJumpheight = 6f;
//[SerializeField] float m_maxJumpScalar = 1.5f;
//[SerializeField] float m_timeToReachApex = 1f;
//[SerializeField] float m_jumpChargeTime = 2f;
//float m_currentJumpHeight;
//float m_maxJumpHeight;//Will be initialized during Start()

////Horizontal force properties
//[Header("Horizontal Input")]
//[SerializeField] float m_timetoReachMaxSpeedFromInput = 4f;
////Quick turn

////Gravity properties
//[Header("Gravity")]
//[SerializeField] float m_gravityScalar = 1f;
//[SerializeField] float m_timeToReachTerminalVelocity = 1f;
//[SerializeField] float m_terminalVelocity = 22f;
//[SerializeField] float m_currentVerticalSpeed;

//[Header("Stunned")]
//[Range(0.3f, 1.5f)]
//[SerializeField]
//private float m_stunnedTime = 1f;
//[SerializeField]
//private float m_pushBackForce;
//float m_stunTimerHandler;


[SerializeField] AnimationCurve hurtFlash;

    bool m_bounceless = false;

    //Radar properties
    float m_radarRadius = 5f;

    //Ground Buffer
    float m_groundBufferTime = 0.1f;
    float m_groundBufferHandler;

    //Sensor properties
    [HideInInspector]
    public struct ActiveSensors
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
    [HideInInspector]
    public ActiveSensors m_sensors;
    float m_minSensorLength = 2f;
    float m_rbVelocityPercetange = 0.25f;

    HashSet<RaycastHit2D> m_sensorBuffer = new HashSet<RaycastHit2D>();

    ////Layers or axis
    //const string OBSTACLE = "Obstacle";
    //const string THREAT = "Threat";

    //States
    enum BouncyState
    {
        FREE_ROAMING,
        CHAINED_ATTACK,
        STUNNED
    }

    BouncyState m_CurrentState = BouncyState.FREE_ROAMING;

    SpriteRenderer sr;
    ParticleSystem ps;
    ValueBar vb;
    int collisionCounter = 0;

    [Header("Analytics")]
    //Analytics stuff, no touchie
    [SerializeField] AnalyticsEventTracker bounceEvent;
    [SerializeField] AnalyticsEventTracker chainEvent;
    [SerializeField] AnalyticsEventTracker stunEvent;
    public string bounceName;
    public float bounceUpwardVelocity;
    public float bounceHorizontalVelocity;
    public List<string> chainEnemies;
    public string chain;
    public string stunName;
    AnalyticsConfig aC;
    


    // Start is called before the first frame update
    void Start()
    {
       
        ////Set initial player state
        //m_CurrentState = BouncyState.FREE_ROAMING;

        ////Set jump limitations
        //m_maxJumpHeight = m_minJumpheight * m_maxJumpScalar;
        //m_currentJumpHeight = m_minJumpheight;

        aC = FindObjectOfType<AnalyticsConfig>();
        chainEnemies = new List<string>();

    }

    // Update is called once per frame
    void Update()
    {
        
        ////this isn't the best way to do this but the script swapping makes it awkward
        //if (m_animator.GetBool("ballMode") == false)
        //{
        //    m_animator.SetBool("ballMode", true);
        //}

        ////Fire sensors that objext surroundings
        //SetSensors();

        //m_currentHorizontalSpeed = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxHorizontalSpeed);
        //m_currentVerticalSpeed = Mathf.Clamp(m_currentVerticalSpeed, -m_terminalVelocity, m_terminalVelocity);

        //if (m_CurrentState == BouncyState.FREE_ROAMING)
        //{
        //    CheckPlayerHorizontalInput();
        //}

        ////Is the player still stunned? 
        //if (Time.time > m_stunTimerHandler && m_CurrentState == BouncyState.STUNNED)
        //{
        //    m_CurrentState = BouncyState.FREE_ROAMING;
        //}
        ////Are we currenty supposed to be grounded?
        //RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, m_capsuleCollider.size.y / 2 + 0.3f, LayerMask.GetMask(OBSTACLE));
        //m_grounded = groundHit;

        //ApplyGravityIfNotGrounded();
        //CheckJumpStatus();

        //sr.color = Color.Lerp(Color.white, Color.red, hurtFlash.Evaluate(m_stunTimerHandler - Time.time));
    }

    private void FixedUpdate()
    {
        ////Synchronize transform with rigibody
        //transform.position = m_rb.position;

        //if (m_CurrentState == BouncyState.FREE_ROAMING)
        //{
        //    //Calculate velocity from force accumulated from input and jump
        //    m_currentHorizontalVelocity = m_lastInputDirection * m_currentHorizontalSpeed;
        //    m_currentVerticallVelocity = new Vector2(0, m_currentVerticalSpeed);
        //    m_currentVelocity = m_currentHorizontalVelocity + m_currentVerticallVelocity;
        //}
        //else if (m_CurrentState == BouncyState.CHAINED_ATTACK)
        //{
        //    //Not used atm
        //    m_currentVelocity = m_currentVelocity.normalized * (m_currentHorizontalSpeed + Mathf.Abs(m_currentVerticalSpeed));
        //}
        //else if (m_CurrentState == BouncyState.STUNNED)
        //{
        //    //Ignore player input as long as player is stunned
        //    m_currentVelocity = new Vector2(0, m_currentVerticalSpeed);
        //}


        //m_rb.velocity = m_currentVelocity;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (m_rb)
        //{
        //    //if (collision.GetComponent<Enemy_Base>())
        //    //{
        //    //    Destroy(collision.gameObject);
        //    //    CheckEnemyRadar();
        //    //}

        //    //if (m_CurrentState == BouncyState.FREE_ROAMING)
        //    //{
        //    //    if (m_rb.velocity.y < 0 && m_sensors.DSensor)
        //    //    {
        //    //        //m_grounded = true;

        //    //        m_lastPositionAfterHittinGround = m_rb.position;
        //    //    }
        //    //    ReactToBorders();
        //    //    collisionCounter++;
        //    //    //Debug.Log("Collision #: " + collisionCounter);
        //    //    //Debug.Log(collision.gameObject.name);
        //    //}

        //    //Did we hit an enemie's weakspot or wall? 
        //    if (collision.GetComponent<Enemy_Base>() || collision.tag == OBSTACLE || collision.gameObject.layer == LayerMask.NameToLayer(OBSTACLE))
        //    {

        //        if (m_rb.velocity.y < 0 && m_sensors.DSensor)
        //        {
        //            //Record the last position when hitting the ground
        //            m_lastPositionAfterHittinGround = m_rb.position;
        //        }
        //        Enemy_Base hitEnemy = collision.GetComponent<Enemy_Base>();
        //        if (hitEnemy)
        //        {

        //            if (collision.GetComponent<Respawnable>())
        //            {
        //                collision.gameObject.GetComponent<Respawnable>().Die();
        //            }
        //            else
        //            {
        //                Destroy(collision.gameObject);
        //            }

        //        }
        //        ReactToBorders(hitEnemy);
        //        collisionCounter++;

        //    }
        //    //Did we hit anything that threatens the player?
        //    else if (collision.gameObject.layer == LayerMask.NameToLayer(THREAT) && m_CurrentState != BouncyState.STUNNED)
        //    {
        //        //TODO take damage
        //        m_CurrentState = BouncyState.STUNNED;
        //        m_currentHorizontalSpeed = 0;
        //        m_stunTimerHandler = Time.time + m_stunnedTime;

        //        stunName = collision.gameObject.name;
        //        if (aC.gathering)
        //        {
        //            stunEvent.TriggerEvent();
        //            if (aC.debug) print("Stun Event fired: " + stunName);
        //        }
        //        else if (aC.debug)
        //        {
        //            print("Stun Event not fired: " + stunName);
        //        }

        //    }
        //}


    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (m_grounded && m_currentVerticalSpeed > 0) { m_grounded = false; }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void ApplyGravityIfNotGrounded()
    {
        //TODO when happy with testing values, move initialization to start, to avoid uneccessary operations

        //Constant rate of change for gravity pulling player down
        //float gravityAccRate = (m_terminalVelocity / m_timeToReachTerminalVelocity) * m_gravityScalar;
        //gravityAccRate *= Time.deltaTime;
        //if (!m_grounded && Time.time > m_groundBufferHandler)
        //{
        //    m_currentVerticalSpeed -= gravityAccRate;
        //    m_bounceless = false;
        //}

        ////Stop bouncing if player is moving vertically slowly enough and player intention is to be grounded
        //if (Mathf.Abs(m_currentVerticalSpeed) < gravityAccRate * 10f && m_grounded && m_lastPositionAfterHittinGround != Vector2.zero)
        //{
        //    m_grounded = true;
        //    m_bounceless = true;
        //    m_currentVerticalSpeed = 0;

        //    if (m_bounceless)
        //    {
        //        m_rb.position = new Vector2(m_rb.position.x, m_lastPositionAfterHittinGround.y + 0.15f);
        //    }
        //    //Reset handler
        //    m_groundBufferHandler = Time.time + m_groundBufferTime;
        //}
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //private void BounceTowardsEnemyInRadar(Transform closestValidEnemy = null)
    //{
    //    if (closestValidEnemy)
    //    {
    //        m_CurrentState = BouncyState.CHAINED_ATTACK;

    //        Vector2 closestEnemyPosition = new Vector2(closestValidEnemy.position.x, closestValidEnemy.position.y);
    //        m_currentVelocity = (closestEnemyPosition - m_rb.position).normalized;
    //    }
    //}
    //private void CheckEnemyRadar()
    //{
    //    //Use a sphere cast to find all enemies within player radar
    //    Collider2D[] enemiesInRadar = Physics2D.OverlapCircleAll(m_rb.position, m_radarRadius, LayerMask.GetMask(THREAT));
    //    if (enemiesInRadar.Length == 0)
    //    {
    //        m_CurrentState = BouncyState.FREE_ROAMING;
    //        return;
    //    }
    //    HashSet<GameObject> validEnemies = new HashSet<GameObject>();//If there is no obstacle between the player and enemy, then it is a valid path

    //    //Find all valid enemies in radar
    //    foreach (Collider2D enemy in enemiesInRadar)
    //    {
    //        Vector2 enemyPosition2D = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
    //        Vector2 pathTowardsEnemy = enemyPosition2D - m_rb.position;

    //        RaycastHit2D hitObstacle = Physics2D.Raycast(m_rb.position, pathTowardsEnemy.normalized, pathTowardsEnemy.magnitude, LayerMask.GetMask(OBSTACLE));
    //        if (!hitObstacle) { validEnemies.Add(enemy.gameObject); }
    //    }

    //    float currentClosestDistance = Mathf.Infinity;
    //    Transform closestValidEnemy = null;
    //    //Go through valid enemies if any, to find closest enemy
    //    if (validEnemies.Count > 0)
    //    {
    //        foreach (GameObject enemy in validEnemies)
    //        {
    //            Vector2 enemyPosition2D = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
    //            float distanceToEnemy = Vector2.Distance(enemyPosition2D, m_rb.position);

    //            if (distanceToEnemy < currentClosestDistance)
    //            {
    //                closestValidEnemy = enemy.transform;
    //                currentClosestDistance = distanceToEnemy;
    //            }
    //        }
    //    }

    //    //Check if there is a valid path to closest enemy
    //    if (closestValidEnemy != null) { BounceTowardsEnemyInRadar(closestValidEnemy); }
    //}
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------




    //private void CheckJumpStatus()
    //{
    //    //Jump if player is grounded and released input
    //    if (m_grounded && InputManager.WasJumpPressed())
    //    {
    //        //TODO Adjustable jump scalar for when jump is held and then released
    //        m_currentVerticalSpeed = (2 * m_currentJumpHeight) / m_timeToReachApex;
    //        m_grounded = false;
    //        m_currentJumpHeight = m_minJumpheight;
    //    }
    //    else if (m_grounded && InputManager.JumpHeld())//Variable jump height
    //    {
    //        float jumpAcc = ((m_maxJumpHeight - m_minJumpheight) / m_jumpChargeTime) * Time.deltaTime;
    //        m_currentJumpHeight += jumpAcc;

    //        m_currentJumpHeight = Mathf.Clamp(m_currentJumpHeight, m_minJumpheight, m_maxJumpHeight);



    //        if (m_currentJumpHeight == m_maxJumpHeight && ps.isEmitting == false)
    //        {
    //            ps.Play();
    //        }
    //        else
    //        {
    //            vb.Display(m_currentJumpHeight / m_maxJumpHeight);
    //        }
    //    }
    //    else
    //    {
    //        ps.Stop();
    //        vb.Hide();
    //    }

    //}
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //private void CheckPlayerHorizontalInput()
    //{
    //    //Debug.Log(m_currentVelocity);
    //    if (IsQuickturning())
    //    {
    //        float quickTurnRate = (m_maxHorizontalSpeed / m_timetoReachMaxSpeedFromInput) * Time.deltaTime;
    //        m_currentHorizontalSpeed -= quickTurnRate;
    //    }
    //    else if (InputManager.PressingMovementInput())
    //    {
    //        float horizontalAcc = (m_maxHorizontalSpeed / m_timetoReachMaxSpeedFromInput) * Time.deltaTime;

    //        m_currentHorizontalSpeed += horizontalAcc;
    //        m_lastInputDirection = InputManager.GetMovementInput();

    //    }

    //}

    //private bool IsQuickturning()
    //{
    //    //Quick turn as long as player drastically changed direction and still has some horizontal speed being applied
    //    return InputManager.PressingMovementInput() && m_lastInputDirection != InputManager.GetMovementInput() && m_currentHorizontalSpeed > 0;
    //}
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

   public bHitResults ReactToBorders(Collider2D colliderHit)

    {
        MergedPlayerController merged = GetComponent<MergedPlayerController>();
        bHitResults result = new bHitResults();
        if (!m_bounceless)
        {
            foreach (RaycastHit2D sensorHit in m_sensorBuffer)
            {

                string obstacleName = colliderHit.transform.name;
                string sensorHitName = sensorHit.transform.name;
                if (obstacleName == sensorHitName)
                {
                    if (sensorHit == m_sensors.LSensor || sensorHit == m_sensors.RSensor)
                    {
                        result.bounceHorizontally = true;
                    }
                    else
                    {
                        result.bounceVertically = true;
                    }
                }
                
            }
        }
        //    //Up or down sensor were hit
        //    if (m_sensors.USensor || m_sensors.DSensor)
        //    {
        //        //Bounce a bit less everytime
        //        //m_currentVerticalSpeed = hitENemy ? m_currentVerticalSpeed * -1.5f : m_currentVerticalSpeed * -0.75f;

        //        result.bounceVertically = true;


        //bounceName = hitENemy.name;
        //bounceUpwardVelocity = m_currentVerticalSpeed;
        //bounceHorizontalVelocity = m_currentHorizontalSpeed; //vars


        //    result.bounceHorizontally = true;
        //#region bounce analytics
        //

        //    bounceName = hitENemy.name;
        //    bounceUpwardVelocity = m_currentVerticalSpeed;


        //    chainEnemies.Add(bounceName);
        /*if (hitENemy)
            {
            bounceEvent.TriggerEvent();
                if (aC.debug) print("bounce event fired: " + bounceName + " at " + bounceUpwardVelocity);
            }
            else if (aC.debug)
            {
                print("bounce event not fired: " + bounceName + " at " + bounceUpwardVelocity);
            }
        
        else
        {
            m_grounded = merged.m_grounded;
            if (m_grounded)
            {
                if (chainEnemies.Count > 1)
                {*/

        //    if (aC.gathering)
        //    {

        //        bounceEvent.TriggerEvent();
        //        if (aC.debug) print("bounce event fired: " + bounceName + " at " + bounceUpwardVelocity);
        //    }
        //    else if (aC.debug)
        //    {
        //        print("bounce event not fired: " + bounceName + " at " + bounceUpwardVelocity);
        //    }
        //}
        //else
        //{
        //    if (m_grounded)
        //    {
        //        if (chainEnemies.Count > 1)
        //        {

        //            chain = "";
        //            for (int i = 0; i < chainEnemies.Count; i++)
        //            {
        //                chain += chainEnemies[i];
        //                chain += " ";
        //            }

        //            if (aC.debug)
        //            {
        //                if (aC.gathering)
        //                {
        //                    print("chain event fired: " + chain);
        //                }
        //                else
        //                {
        //                    print("chain event not fired: " + chain);
        //                }
        //            }

        //        }
        //        else if (aC.debug)
        //        {
        //            print("no chain");
        //        }

        //        chainEnemies.Clear();
        //    }
        //}

        //#endregion
        return result;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //private Sensors GetSensorHit()
    //{

    //}
    public void SetSensors()
    {
        m_sensorBuffer.Clear();

        m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, m_minSensorLength, LayerMask.GetMask(OBSTACLE, THREAT));
        m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, m_minSensorLength, LayerMask.GetMask(OBSTACLE, THREAT));
        m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up, m_minSensorLength, LayerMask.GetMask(OBSTACLE, THREAT));
        m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));
        m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right, m_minSensorLength, LayerMask.GetMask(OBSTACLE, THREAT));
        m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));

        UpdateSensorHits(m_sensors.USensor);
        UpdateSensorHits(m_sensors.DSensor);
        UpdateSensorHits(m_sensors.LSensor);
        UpdateSensorHits(m_sensors.RSensor);


        //float sensorLength = m_rb.velocity.magnitude * m_rbVelocityPercetange;
        //sensorLength = Mathf.Clamp(sensorLength, m_minSensorLength, m_minSensorLength + sensorLength);
        //m_sensors.USensor = Physics2D.Raycast(transform.position, transform.up, sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.URSensor = Physics2D.Raycast(transform.position, (transform.up + transform.right).normalized, m_minSensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RSensor = Physics2D.Raycast(transform.position, transform.right, sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.RDSensor = Physics2D.Raycast(transform.position, (-transform.up + transform.right).normalized,sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DSensor = Physics2D.Raycast(transform.position, -transform.up,sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.DLSensor = Physics2D.Raycast(transform.position, (-transform.up + -transform.right).normalized,sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LSensor = Physics2D.Raycast(transform.position, -transform.right,sensorLength, LayerMask.GetMask(OBSTACLE));
        //m_sensors.LUSensor = Physics2D.Raycast(transform.position, (transform.up + -transform.right).normalized, sensorLength, LayerMask.GetMask(OBSTACLE));
    }

    void UpdateSensorHits(RaycastHit2D hitDetected)
    {
        if (!hitDetected) return;

        if (!m_sensorBuffer.Contains(hitDetected))
        {
            m_sensorBuffer.Add(hitDetected);
        }
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

    //protected override bool IsGrounded() { return m_grounded; }
    //protected override void ResetVelocity()
    //{
    //    base.ResetVelocity();
    //    m_currentHorizontalSpeed = 0;
    //    m_lastInputDirection = Vector2.zero;


    //    m_currentVerticalSpeed = 0;

    //}
}