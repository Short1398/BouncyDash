using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergedPlayerController : PlayerController_Base
{
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


    const string BALLMODE = "ballMode";
    const string SPEED = "speed";

    //Controller components
    BouncyController m_bController;
    WalkController m_walkController;
    //Other components
    CapsuleCollider2D m_capsuleCollider;
    SpriteRenderer m_sr;
    ParticleSystem m_ps;
    ValueBar m_vb;

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
        UpdateCurrentController();
    }

    void InitializePlayerController()
    {
        m_currentBouncyState = BouncyStates.FREE_ROAMING;

        //Get controllers component reference
        m_bController = GetComponent<BouncyController>();
        m_walkController = GetComponent<WalkController>();
        //Get other components
        m_sr = GetComponentInChildren<SpriteRenderer>();
        m_ps = GetComponentInChildren<ParticleSystem>();
        m_vb = GetComponentInChildren<ValueBar>();
        m_capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void UpdateCurrentController()
    {
        UpdateAnimation();
        if (m_currentController == PlayerControllers.DEFAULT)
        {

        }
        else if (m_currentController == PlayerControllers.BOUNCY)
        {
            
        }

    }

    private void FixedUpdate()
    {
        
    }

    void UpdateAnimation()
    {
        bool ballMode = m_currentController == PlayerControllers.BOUNCY ? true : false;
        m_animator.SetBool(BALLMODE, ballMode);

        if (m_currentController == PlayerControllers.DEFAULT)
        {
            m_animator.SetFloat(SPEED, Mathf.Abs(m_lastCalculatedVelocity.x));
        }
    }
}
