using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(WalkController), typeof(WalkController))]
public class MergedPlayerController : PlayerController_Base
{
    [Header("Bouncy Jump Properties")]
    [SerializeField]
    private float m_bminJumpHeight = 5f;
    [SerializeField]
    private float m_jumpBufferminDistance = 1f;

    [Header("Bouncy Horizontal Properties")]
    [SerializeField]
    private float m_maxBHorizontalSpeed;
    [SerializeField]
    private float m_btimeToReachMaxSpeed;

    [Header("Bouncing off enemies")]
    [Range(1, 4)]
    [SerializeField]
    private float m_enemyBounceScalar = 1.7f;

    [Header("Swap Properties")]
    [SerializeField]
    private float m_swapCooldown = 1f;

    [Header("Audio Sources")]
    //Audio Sources
    [SerializeField]
    private AudioSource grubSound;
    [SerializeField]
    private AudioSource beetleSound;
    [SerializeField]
    private AudioSource waspSound;
    [SerializeField]
    private AudioSource jumpSound;
    [SerializeField]
    private AudioSource hurtSound;
    [SerializeField]
    private AudioSource moveSound;

    public enum PlayerControllers
    {
        BOUNCY,
        DEFAULT,
        STUNNED
    }
    PlayerControllers m_currentController;

    public enum BouncyStates
    {
        FREE_ROAMING,
        CHAINED_ATTACK
    }
    BouncyStates m_currentBouncyState;

    public struct TimerHandlersWrapper
    {
        public float swapHandler;
    }

    TimerHandlersWrapper m_handlersWrapper;

    Vector2 hitPoint;


    const string BALLMODE = "ballMode";
    const string SPEED = "speed";

    Vector2 m_lastPositionAfterHittingGround = Vector2.zero;

    bool m_placeOnGroundFlag = false;
    bool m_bounceless = true;

    //Controller components
    BouncyController m_bController;
    WalkController m_walkController;
    //Other components
    CapsuleCollider2D m_capsuleCollider;
    SpriteRenderer m_sr;
    ParticleSystem m_ps;
    Vavi m_vb;

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
        //Initialize parent controller
        base.Start();

        InitializePlayerController();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_rb) throw new MissingComponentException(gameObject.name + " is missing the rigibody component, all movement relies on the rigidbody component");

        UpdateCurrentController();


        //HARD CODED FIX SORRY
        if (m_rb.position.y < -16)
        {
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
            m_currentVerticalSpeed = 0;
        }
    }

    void InitializePlayerController()
    {
        m_currentController = PlayerControllers.DEFAULT;
        m_currentBouncyState = BouncyStates.FREE_ROAMING;

        aC = FindObjectOfType<AnalyticsConfig>();

        //Get controllers component reference
        m_bController = GetComponent<BouncyController>();
        m_walkController = GetComponent<WalkController>();
        //Get other components
        m_sr = GetComponentInChildren<SpriteRenderer>();
        m_ps = GetComponentInChildren<ParticleSystem>();
        m_vb = Vavi.GetVavi(1);
        m_vb.Show(false);
        m_capsuleCollider = GetComponent<CapsuleCollider2D>();

        m_grounded = false;

        m_maxJumpHeight = m_bminJumpHeight * m_maxJumpScalar;

        //Audio Sources
        AudioSource[] allAudioSources = GetComponents<AudioSource>();
        grubSound = allAudioSources[0];
        beetleSound = allAudioSources[1];
        waspSound = allAudioSources[2];
        jumpSound = allAudioSources[3];
        hurtSound = allAudioSources[4];
        moveSound = allAudioSources[5];

    }

    void UpdateCurrentController()
    {

        //Are we currenty supposed to be grounded?
        float castDistance = m_grounded ? (m_capsuleCollider.size.y / 2) + 0.6f : Mathf.Abs(m_rb.velocity.y);



        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, castDistance, LayerMask.GetMask(OBSTACLE));


        if (groundHit && m_rb.velocity.y < 0)
        {
            m_placeOnGroundFlag = true;
            hitPoint = groundHit.point;
        }
        m_grounded = groundHit;



        UpdateSwapStatus();
        UpdateAnimation();
        UpdateCollider();

        //Check grounded status

        ClampSpeedForces();
        UpdateHorizontalMovement();

        CheckJumpStatus();
        if (!m_grounded)
        {
            ApplyGravityIfNotGrounded();
        }

        if (m_currentController == PlayerControllers.DEFAULT)
        {
            bool triedDashing = m_walkController.DashAtttempted();
            UpdateDash(triedDashing);
            //Collision polish
        }
        else if (m_currentController == PlayerControllers.BOUNCY)
        {
            m_bController.SetSensors();
        }
        else if (m_currentController == PlayerControllers.STUNNED)
        {
            UpdateStunnedState();
        }

         //if(m_currentController == PlayerControllers.BOUNCY)Debug.Log(m_currentVerticalSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (m_rb)
        {
            float castDistance = m_grounded ? (m_capsuleCollider.size.y / 2) + 0.6f : Mathf.Abs(m_rb.velocity.y);
            float endPoint = transform.position.y - castDistance;
            Vector2 endVector = transform.position + new Vector3(0, endPoint);
            Gizmos.DrawLine(transform.position, endVector);
        }

    }

    private void FixedUpdate()
    {
        if (!m_rb) {
            print("not working anyways");
            return;
        } 

        transform.position = m_rb.position;
        if (m_currentController != PlayerControllers.STUNNED)
        {
            //TODO find a clearer and more efficient horiztonal infinity check
            if (Mathf.Infinity - Mathf.Abs(m_currentHorizontalSpeed) >= 10f)//I absolutely hate this edge case and hardcode solution, sorry guys
            {
                //update horizontal and vertical velocity
                m_currentHorizontalVelocity = m_lastInputDirection * m_currentHorizontalSpeed;
                m_currentVerticalVelocity = Vector2.up * (m_currentVerticalSpeed);

                //if (m_currentController == PlayerControllers.BOUNCY) Debug.Log(m_currentVerticalVelocity);


                m_currentTotalVelocity = m_currentHorizontalVelocity + m_currentVerticalVelocity;

                m_rb.velocity = m_currentTotalVelocity;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_rb)
        {
            //Did we hit an enemie's weakspot or wall? 
            if (collision.GetComponent<Enemy_Base>() || collision.tag == OBSTACLE || collision.gameObject.layer == LayerMask.NameToLayer(OBSTACLE))
            {

                if (m_rb.velocity.y < 0 && m_bController.m_sensors.DSensor)
                {
                    //Record the last position when hitting the ground
                    m_lastPositionAfterHittingGround = m_rb.position;
                }
                Enemy_Base hitEnemy = collision.GetComponent<Enemy_Base>();

                CheckBorderReaction(collision, hitEnemy);


                if (hitEnemy)
                {
                    //Did we hit a grub enemy?
                    if (collision.GetComponent<WalkingEnemy>())
                    {
                        grubSound.Play();
                    }
                    //Did we hit a beetle enemy?
                    if (collision.GetComponent<TurretController>())
                    {
                        beetleSound.Play();
                    }
                    //Did we hit a hornet enemy?
                    if (collision.GetComponent<HornetController>())
                    {
                        waspSound.Play();
                    }

                    if (collision.GetComponent<Respawnable>())
                    {
                        collision.gameObject.GetComponent<Respawnable>().Die();
                    }
                    else
                    {
                        Destroy(collision.gameObject);
                    }

                }

            }
            //Did we hit anything that threatens the player?
            else if (collision.gameObject.layer == LayerMask.NameToLayer(THREAT) && m_currentController != PlayerControllers.STUNNED)
            {
                //Play audio for when player takes damage
                //hurtSound.Play();

                //TODO take damage
                m_currentController = PlayerControllers.STUNNED;
                m_currentHorizontalSpeed = 0;
                
                m_stunTimerHandler = Time.time + m_stunnedTime;

                stunName = collision.gameObject.name;
                if (aC.gathering)
                {
                    stunEvent.TriggerEvent();
                    if (aC.debug) print("Stun Event fired: " + stunName);
                }
                else if (aC.debug)
                {
                    print("Stun Event not fired: " + stunName);
                }

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(OBSTACLE) && m_placeOnGroundFlag)
        {
            m_rb.position = new Vector2(m_rb.position.x, hitPoint.y + .7f);
            m_currentVerticalSpeed = 0;
            m_placeOnGroundFlag = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer(THREAT))
        {
            m_currentController = PlayerControllers.STUNNED;
            m_currentHorizontalSpeed = 0;
            m_stunTimerHandler = Time.time + m_stunnedTime;

        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //Methods for both components
    void UpdateSwapStatus()
    {
        if (m_currentController == PlayerControllers.STUNNED) return;

        bool canSwap = m_handlersWrapper.swapHandler < Time.time && InputManager.SwapPressed();
        if (canSwap)
        {
            m_currentController = m_currentController == PlayerControllers.DEFAULT ? PlayerControllers.BOUNCY : PlayerControllers.DEFAULT;
            //Animator controller param update
            bool ballMode = m_currentController == PlayerControllers.BOUNCY ? true : false;
            m_animator.SetBool(BALLMODE, ballMode);

            m_handlersWrapper.swapHandler = Time.time + m_swapCooldown;
        }
    }
    void UpdateAnimation()
    {
        //Sprite renderer update
        m_sr.flipX = m_currentTotalVelocity.x > 0 ? true : false;


        if (m_currentController == PlayerControllers.DEFAULT)
        {
            m_animator.SetFloat(SPEED, Mathf.Abs(m_lastCalculatedVelocity.x));
        }
    }

    void UpdateCollider()
    {
        m_capsuleCollider.isTrigger = m_currentController == PlayerControllers.BOUNCY ? true : false;
    }

    void ClampSpeedForces()
    {
        float bXClamp = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxBHorizontalSpeed);//Bouncy horiztonal clamp
        float dXClamp = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxDefaultSpeed);//Default horizontal clamp
        float dashingClamp = Mathf.Clamp(m_currentHorizontalSpeed, 0, m_maxDashSpeed);

        m_currentHorizontalSpeed = m_currentController == PlayerControllers.DEFAULT ? dXClamp : bXClamp;
        m_currentHorizontalSpeed = m_isDashing ? dashingClamp : m_currentHorizontalSpeed;

        float maxInitialVelocityFromJump = (2 * m_currentJumpHeight) / m_timeToReachApex;
        m_currentVerticalSpeed = Mathf.Clamp(m_currentVerticalSpeed, -m_terminalVelocity, maxInitialVelocityFromJump);
    }

    void UpdateHorizontalMovement()
    {
        if (m_isDashing) return;
        float currentMaxSpeed = m_currentController == PlayerControllers.DEFAULT ? m_maxDefaultSpeed : m_maxBHorizontalSpeed;
        float currentTimeToReachTarget = m_currentController == PlayerControllers.DEFAULT ? m_timeToReachMaxSpeed : m_btimeToReachMaxSpeed;
        float roc = (currentMaxSpeed / currentTimeToReachTarget) * Time.deltaTime;//Rate of change

        bool QuickTurning = InputManager.PressingMovementInput() && InputManager.GetMovementInput() != m_lastInputDirection && m_currentHorizontalSpeed > 0;
        if (QuickTurning)
        {
            m_currentHorizontalSpeed -= roc;

        }
        else if (InputManager.PressingMovementInput())
        {
            m_currentHorizontalSpeed += roc;
            m_lastInputDirection = InputManager.GetMovementInput();
        }
        else
        {
            m_currentHorizontalSpeed -= (roc / 3f);
        }

    }

    void CheckJumpStatus()
    {
        bool jumpStatus = m_currentController == PlayerControllers.DEFAULT ? false : true;
        bool canJump = (m_grounded && InputManager.WasJumpPressed(jumpStatus));
        bool jumpHeld = (m_grounded && InputManager.JumpHeld());
        bool isBouncy = m_currentController == PlayerControllers.BOUNCY;

        float jumpHeight = isBouncy ? m_currentJumpHeight : m_minJumpheight;

        bool readyToJump = m_currentVerticalSpeed == 0;

        if (readyToJump)
        {
            if (canJump)
            {
                m_currentVerticalSpeed = (2 * jumpHeight) / m_timeToReachApex;
                m_grounded = false;
                m_currentJumpHeight = isBouncy ? m_currentVerticalSpeed : m_minJumpheight;
                m_bounceless = false;

                //Play jump audio
                jumpSound.Play();
            }
            else if (isBouncy && jumpHeld)
            {
                //TODO optimize the deltaTime calculation
                float jumpAcc = ((m_maxJumpHeight - m_bminJumpHeight) / m_jumpChargeTime) * Time.fixedDeltaTime;

                m_currentJumpHeight += jumpAcc;

                m_currentJumpHeight = Mathf.Clamp(m_currentJumpHeight, m_bminJumpHeight, m_maxJumpHeight);


                bool atJumpPeek = (m_currentJumpHeight >= m_maxJumpHeight && !m_ps.isEmitting);
                float alpha = m_currentJumpHeight / m_maxJumpHeight;

                if (atJumpPeek)
                {
                    m_vb.Show(false);
                    m_ps.Play();
                }
                else
                {
                    m_vb.ValueSet(alpha);
                    m_vb.Show(true);
                }
            }
            else
            {
                m_ps.Stop();
                m_vb.Show(false);
            }
        }
    }

    void CheckBorderReaction(Collider2D colliderHit, Enemy_Base hitEnemy = null)
    {
        if (m_currentController != PlayerControllers.BOUNCY) return;

        m_bounceResult = m_bController.ReactToBorders(colliderHit);

        #region Bounce and Chain Analytics

        if (hitEnemy)
        {
            //bounce
            bounceName = hitEnemy.name;
            bounceUpwardVelocity = m_currentVerticalSpeed;
            bounceHorizontalVelocity = m_currentHorizontalSpeed;
            //chain
            chainEnemies.Add(bounceName);

            if (aC.gathering)
            {
                bounceEvent.TriggerEvent();
                if (aC.debug) print("bounce event fired: " + bounceName + " at " + bounceUpwardVelocity);
            }
            else if (aC.debug)
            {
                print("bounce event not fired: " + bounceName + " at " + bounceUpwardVelocity);
            }
        }
        else
        {
            if (m_grounded)
            {
                if (chainEnemies.Count > 1)
                {
                    chain = "";
                    for (int i = 0; i < chainEnemies.Count; i++)
                    {
                        chain += chainEnemies[i];
                        chain += " ";
                    }

                    if (aC.gathering)
                    {
                        if (aC.debug)
                        {
                            chainEvent.TriggerEvent();
                            print("chain event fired: " + chain);
                        }

                    }
                    else if (aC.debug)
                    {
                        print("chain event not fired: " + chain);
                    }

                }
                else if (aC.debug)
                {

                    print("no chain");

                    chainEnemies.Clear();

                }
            }
        }

        #endregion

        if (hitEnemy)
        {

            //Debug.Log("horizontal " + m_bounceResult.bounceHorizontally + " on " + hitEnemy);
            //Debug.Log("vertical " + m_bounceResult.bounceVertically + " on " + hitEnemy);
        }

        else
        {
            //print("not enemy");
        }

        
        if (m_bounceResult.bounceHorizontally)
        {
            m_currentHorizontalSpeed = hitEnemy ? m_currentHorizontalSpeed += 2 : m_currentHorizontalSpeed;
            m_lastInputDirection.x *= -1;
        }
        if (m_bounceResult.bounceVertically)
        {
            //Bounce a bit less everytime
            m_currentVerticalSpeed = hitEnemy ? m_currentVerticalSpeed * -m_enemyBounceScalar : m_currentVerticalSpeed * -0.6f;

            m_lastPositionAfterHittingGround = colliderHit.transform.position;

            m_currentVerticalSpeed = Mathf.Abs(m_currentVerticalSpeed) < 1.4f ? 0 : m_currentVerticalSpeed;
        }

        m_currentJumpHeight = m_currentController == PlayerControllers.DEFAULT ? m_minJumpheight : m_bminJumpHeight;
    }


void ApplyGravityIfNotGrounded()
{
    //TODO when happy with testing values, move initialization to start, to avoid uneccessary operations

    //Constant rate of change for gravity pulling player down
    float gravityAccRate = (m_terminalVelocity / m_timeToReachTerminalVelocity) * m_gravityScalar;
    gravityAccRate *= Time.deltaTime;

    float distanceToGround = Vector2.Distance(transform.position, m_lastPositionAfterHittingGround);
    //m_bounceless = Mathf.Abs(m_currentVerticalSpeed) < gravityAccRate * 3f && distanceToGround <= m_jumpBufferminDistance;
    if (!m_grounded)
    {
        m_currentVerticalSpeed -= gravityAccRate;
        //m_bounceless = false;
    }

    //Stop bouncing if player is moving vertically slowly enough and player intention is to be grounded
    if (m_bounceless && m_currentController == PlayerControllers.BOUNCY)
    {
        m_grounded = true;
        m_bounceless = true;
        m_currentVerticalSpeed = 0;

        //if (m_bounceless)
        //{
        //    m_rb.position = new Vector2(m_rb.position.x, m_lastPositionAfterHittinGround.y + 0.15f);
        //}
        ////Reset handler
        //m_groundBufferHandler = Time.time + m_groundBufferTime;
    }
}

void UpdateDash(bool dashAttempted = false)
{
    m_isDashing = Time.time > m_timerHandler.dashHandler ? false : true;
    if (dashAttempted && !m_isDashing)
    {
        m_isDashing = true;
        m_currentHorizontalSpeed = m_maxDashSpeed;

        m_timerHandler.dashHandler = Time.time + m_timeToDash;
    }
    else if (m_isDashing)
    {
        float roc = (m_maxDashSpeed / m_timeToDash) * Time.deltaTime;

        m_currentHorizontalSpeed -= roc;
    }
}

void UpdateStunnedState()
{
        bool timesUp = Time.time > m_stunTimerHandler;
        m_currentController = timesUp ? PlayerControllers.DEFAULT : PlayerControllers.STUNNED;
        m_sr.material.color = timesUp ? Color.white : Color.red;
}

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

    //Methods for Bouncy component
    
}