using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    const string HORIZONTALMOV = "Horizontal";
    const string JUMP_BUTTON = "CustomJump";

    public static Vector2 GetMovementInput()
    {
        float horizontalMov = Input.GetAxisRaw(HORIZONTALMOV);

        return new Vector2(horizontalMov,0).normalized;
    }

    public static bool PressingMovementInput()
    {
        return Input.GetButton(HORIZONTALMOV);
    }

    public static bool WasJumpPressed()
    {
        return Input.GetButtonUp(JUMP_BUTTON);
    }

    public static bool JumpHeld()
    {
        return Input.GetButton(JUMP_BUTTON);
    }
}
