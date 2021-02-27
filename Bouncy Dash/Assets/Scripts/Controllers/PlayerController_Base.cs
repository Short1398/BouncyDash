using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Base : MonoBehaviour
{

    // yup

    protected const string HORIZONTALMOV = "Horizontal";
    protected const string JUMP_BUTTON = "Jump";
    protected const string SWAP_BUTTON = "Swap";
    protected const string DASH_BUTTON = "Dash";


    protected Animator a;

    //Swapping properties
    protected float m_swapCooldown = 1f;
    protected float m_swapHandler;
    protected Vector2 m_previousVelocityBeforeSwap;

    protected class VelocityRef
    {
        public Vector2 CurrentVelocity;
    }

    protected VelocityRef m_velocityReference;

    protected bool m_Grounded;

    protected void CheckSwapStatus(PlayerController_Base a, PlayerController_Base b)
    {
        if (InputManager.SwapPressed() && Time.time > m_swapHandler)
        {
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
            m_swapHandler = Time.time + m_swapCooldown;

        }
    }

    protected virtual bool IsGrounded() { return true; }
    protected virtual void ResetVelocity() { }
}