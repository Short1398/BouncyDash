using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    const string HORIZONTALMOV = "Horizontal";
    const string JUMP_BUTTON = "Jump";
    const string SWAP_BUTTON = "Swap";
    const string DASH_BUTTON = "Dash";

    public static float GetAxisDeadZone(string axis)
    {
        return Input.GetAxisRaw(axis);
    }
    public static Vector2 GetMovementInput()
    {
        float horizontalMov = Input.GetAxisRaw(HORIZONTALMOV);

        return new Vector2(horizontalMov,0).normalized;
    }

    public static bool PressingMovementInput()
    {
        return Input.GetButton(HORIZONTALMOV);
    }

    public static bool WasJumpPressed(bool released = true)
    {
        if (!released) { return Input.GetButtonDown(JUMP_BUTTON); }
        return Input.GetButtonUp(JUMP_BUTTON);
    }

    public static bool JumpHeld()
    {
        return Input.GetButton(JUMP_BUTTON);
    }

    public static bool DashPressed() { return Input.GetButtonDown(DASH_BUTTON); }
    public static bool SwapPressed() { return Input.GetButtonDown(SWAP_BUTTON); }

}
