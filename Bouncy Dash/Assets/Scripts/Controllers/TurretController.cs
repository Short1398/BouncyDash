using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : Enemy_Base
{
    [Header("Player Tracking")]
    [SerializeField]
    private float m_minDistanceToPlayer;
    [SerializeField]
    private float m_rotateSpeedInDeg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform playerPos = GameObject.FindGameObjectWithTag(PLAYER_TAG).transform;
        if (playerPos)
        {
            Vector2 playerPos2D = new Vector2(playerPos.position.x, playerPos.position.y);
            Vector2 selfPos2D = new Vector2(transform.position.x, transform.position.y);
            if (Vector3.Distance(playerPos.position, transform.position) < m_minDistanceToPlayer)
            {
                Vector2 vToPlayer = (playerPos2D - selfPos2D).normalized;

                float angleToPlayer = Vector2.Angle(vToPlayer, transform.up);
                float dotProduct = Vector2.Dot(vToPlayer, transform.right);
                float signage = -Mathf.Sign(dotProduct);

                if (angleToPlayer != 0)
                {
                    angleToPlayer = Mathf.Clamp(angleToPlayer, 0f, m_rotateSpeedInDeg);
                    angleToPlayer *= signage * Time.deltaTime;
                    transform.Rotate(0, 0, angleToPlayer, Space.Self);
                }
            }
        }
        else
        {
            throw new UnassignedReferenceException(gameObject.name + " could not find the player");
        }
    }
}
