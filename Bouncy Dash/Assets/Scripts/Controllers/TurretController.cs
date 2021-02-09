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

    [Header("Firing")]
    [SerializeField]
    private Transform m_tipOfBarrel;
    [SerializeField]
    private GameObject m_bulletPrefab;
    [SerializeField]
    private float m_fireRate;
    [SerializeField]
    private float m_bulletSpeed;
    private float m_firingHandler;

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
            if (Vector3.Distance(playerPos.position, transform.position) < m_minDistanceToPlayer)
            {
                RotateTowardsPlayer(playerPos);
                if (Time.time > m_firingHandler)
                {
                    FireAtTarget(playerPos.position);
                    m_firingHandler = Time.time + m_fireRate;
                }
            }
        }
        else
        {
            throw new UnassignedReferenceException(gameObject.name + " could not find the player");
        }
    }

    private void RotateTowardsPlayer(Transform playerPos)
    {
        Vector2 playerPos2D = new Vector2(playerPos.position.x, playerPos.position.y);
        Vector2 selfPos2D = new Vector2(transform.position.x, transform.position.y);

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

    private void FireAtTarget(Vector2 target)
    {
       
        Vector2 selfPos2D = new Vector2(m_tipOfBarrel.position.x, m_tipOfBarrel.position.y);

        Vector2 vToPlayer = (target - selfPos2D).normalized;

        BulletController bullet = Instantiate(m_bulletPrefab, m_tipOfBarrel.position, transform.rotation).GetComponent<BulletController>();
        bullet.BulletConstructor(transform.up, m_bulletSpeed, this.gameObject);
    }
}
