using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Base : MonoBehaviour
{

    [Header("Horizontal Speed")]
    [SerializeField]
    protected float m_timeToReachMaxSpeed;

    //Jump properties
    [Header("Jump")]
    [SerializeField] float m_minJumpheight = 6f;
    [SerializeField] float m_maxJumpScalar = 1.5f;
    [SerializeField] float m_timeToReachApex = 1f;
    [SerializeField] float m_jumpChargeTime = 2f;
    float m_currentJumpHeight;
    float m_maxJumpHeight;//Will be initialized during Start()

    [Header("Gravity")]
    [SerializeField] float m_gravityScalar = 1f;
    [SerializeField] float m_timeToReachTerminalVelocity = 1f;
    [SerializeField] float m_terminalVelocity = 22f;

    [Header("Stunned")]
    [Range(0.3f, 1.5f)]
    [SerializeField]
    private float m_stunnedTime = 1f;
    [SerializeField]
    private float m_pushBackForce;
    float m_stunTimerHandler;

    //Main dynamic forces
    protected float m_currentHorizontalSpeed;
    protected float m_currentVerticalSpeed;

    //Basic direction and position tracking properties
    protected Vector2 m_currentTotalVelocity;
    protected Vector2 m_currentHorizontalVelocity;
    protected Vector2 m_currentVerticalVelocity;
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


    //Components
    protected Animator m_animator;
    protected Rigidbody2D m_rb;


    virtual protected void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        //Set jump limitations
        m_maxJumpHeight = m_minJumpheight * m_maxJumpScalar;
        m_currentJumpHeight = m_minJumpheight;
    }

    //Swapping properties
    protected float m_swapCooldown = 1f;
    protected float m_swapHandler;
    protected Vector2 m_previousVelocityBeforeSwap;

    protected class VelocityRef
    {
        public Vector2 CurrentVelocity;
    }

    protected VelocityRef m_velocityReference;

    protected bool m_grounded;

    protected void CheckSwapStatus(PlayerController_Base a, PlayerController_Base b)
    {
        if (InputManager.SwapPressed() && Time.time > m_swapHandler)
        {
            //Reset velocity for active player controller

            if (a.gameObject.GetComponent<WalkController>().isActiveAndEnabled)
            {
                WalkController controllerRef = a.gameObject.GetComponent<WalkController>();
                controllerRef.ResetVelocity();
            }
            else if (a.gameObject.GetComponent<BouncyController>().isActiveAndEnabled)
            {
                BouncyController controllerRef = a.gameObject.GetComponent<BouncyController>();
                controllerRef.ResetVelocity();
            }




            Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
            brb.velocity = new Vector2(0, brb.velocity.y);

            Game_Manager.SwapControllers(a, b);
            
            m_swapHandler = Time.time + m_swapCooldown;//Activate swap timer
            
        }
    }

    protected virtual bool IsGrounded() { return true; }
    protected virtual void ResetVelocity() { }
}