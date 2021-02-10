using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public static void SwapControllers(PlayerController_Base a , PlayerController_Base b)
    {
        a.enabled = false;
        b.enabled = true;

        Debug.Log("Swapped");
    }
}
