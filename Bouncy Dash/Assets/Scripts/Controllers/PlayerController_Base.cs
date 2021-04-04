using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Base : MonoBehaviour
{

    [Header("Horizontal Speed")]
    [SerializeField]
    protected float m_timeToReachMaxSpeed;
    [SerializeField]
    protected float m_maxDefaultSpeed;

    //Jump properties
    [Header("Jump")]
    [SerializeField] protected float m_minJumpheight = 6f;
    [SerializeField] protected float m_maxJumpScalar = 2f;
    [SerializeField] protected float m_timeToReachApex = 1f;
    [SerializeField] protected float m_jumpChargeTime = 2f;
    protected float m_currentJumpHeight;
    protected float m_maxJumpHeight;//Will be initialized during Start()

    [Header("Gravity")]
    [SerializeField] protected float m_gravityScalar = 1f;
    [SerializeField] protected float m_timeToReachTerminalVelocity = 1f;
    [SerializeField] protected float m_terminalVelocity = 22f;

    [Header("Dashing")]
    [SerializeField] protected float m_timeToDash = 1f;
    [SerializeField] protected float m_maxDashSpeed = 8f;

    [Header("Stunned")]
    [Range(0.3f, 1.5f)]
    [SerializeField]
    protected float m_stunnedTime = 1f;
    [SerializeField]
    private float m_pushBackForce;
    protected float m_stunTimerHandler;

    //Main dynamic forces
    protected float m_currentHorizontalSpeed;
    protected float m_currentVerticalSpeed;

    //Internal floats
    public struct PC_TimerHandlers
    {
        public float dashHandler;
    }
    protected PC_TimerHandlers m_timerHandler;

    //Basic direction and position tracking properties
    protected Vector2 m_currentTotalVelocity;
    public Vector2 m_currentHorizontalVelocity;
    public Vector2 m_currentVerticalVelocity;
    protected Vector2 m_lastInputDirection;
    protected Vector2 m_lastCalculatedVelocity;

    //Input bindings for input manager
    protected const string HORIZONTALMOV = "Horizontal";
    protected const string JUMP_BUTTON = "Jump";
    protected const string SWAP_BUTTON = "Swap";
    protected const string DASH_BUTTON = "Dash";

    //Layers or tag tracking by name
    protected const string OBSTACLE = "Obstacle";
    protected const string THREAT = "Threat";

    //Bool checks
    public bool m_grounded;
    protected bool m_isDashing;

    //Components
    protected Animator m_animator;
    protected Rigidbody2D m_rb;


    virtual protected void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        //m_animator = GetComponent<Animator>();
        m_animator = GetComponentInChildren<Animator>();

        //Set jump limitations
        //m_maxJumpHeight = m_minJumpheight * m_maxJumpScalar;
        m_currentJumpHeight = m_minJumpheight;
    }


    //Hardcode solution to bounvy border check
    [HideInInspector]
    public struct bHitResults
    {
        public bHitResults(bool res1, bool res2)
        {
            bounceHorizontally = res1;
            bounceVertically = res2;
        }
        public bool bounceHorizontally, bounceVertically;
    }
    protected bHitResults m_bounceResult;

    //Swapping properties
    protected Vector2 m_previousVelocityBeforeSwap;

    protected virtual bool IsGrounded() { return true; }
    protected virtual void ResetVelocity() { }
}