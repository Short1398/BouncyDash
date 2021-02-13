using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 CAMERA CONTROLLER
 The Camera Controller MUST be configured individually for each level
    (Or Subsection of level, if it has non-square boundaries,
    in which case there will need to be a trigger that tells the Camera Controller what its new boundaries will be)
 All configuration can and should be done in the inspector, as doing so in code may mess up other levels
 */

public class CameraControl : MonoBehaviour {

    [Tooltip("The bottom left of the screen")]
    public Vector2 minBounds;
    [Tooltip("The top right of the screen")]
    public Vector2 maxBounds;
    [Tooltip("The Player character")]
    public Transform playerPos;

    // Update is called once per frame
    void Update() {
        FollowPlayer();
    }

    void FollowPlayer() {
        if (playerPos.position.x > minBounds.x && playerPos.position.x < maxBounds.x) {
            // Ensure the Camera does not leave horizontal bounds, then smoothly follow the Player
            this.transform.position = new Vector3(playerPos.position.x, this.transform.position.y, -10);
        }
        if (playerPos.position.y > minBounds.y && playerPos.position.y < maxBounds.y) {
            // Ensure the Camera does not leave vertical bounds, then smoothly follow the Player
            this.transform.position = new Vector3(this.transform.position.x, playerPos.position.y, -10);
        }
    }
}
