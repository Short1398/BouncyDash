using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Base : MonoBehaviour
{
   protected const string HORIZONTALMOV = "Horizontal";
   protected const string JUMP_BUTTON = "Jump";
   protected const string SWAP_BUTTON = "Swap";
   protected const string DASH_BUTTON = "Dash";

    //Swapping properties
    protected float m_swapCooldown = 1f;
    protected float m_swapHandler;

    protected void CheckSwapStatus(PlayerController_Base a, PlayerController_Base b)
    {
        if (InputManager.SwapPressed() && Time.time > m_swapHandler)
        {
            Game_Manager.SwapControllers(a, b);
            m_swapHandler = Time.time + m_swapCooldown;
        }
    }
}
