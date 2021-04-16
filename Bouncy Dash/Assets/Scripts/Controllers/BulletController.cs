using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Additional Functionality")]
    [SerializeField]
    private bool DisregardRigidbody = false;
    [SerializeField]
    private bool HasLifetime = false;
    [Range(1, 5)]
    [SerializeField]
    private float lifetime = 1;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private GameObject deathExplosionPrefab;

    //If you want to add new param for the bullets to get a registered hit, put them here and add them to the OnTriggerEnter() function
    [Header("Hit Query Params")]
    [SerializeField]
    private string PLAYER;
    [SerializeField]
    private string ENEMY;
    [SerializeField]
    private string OBSTACLE;
    [SerializeField]
    private string GROUND;

    [Header("Damage")]
    [SerializeField]
    private float m_damage = 1;

    ///Properties will be set when bullet is instantiated
    private Vector2 m_targetDirection;
    private float m_speed;
    private float lifetimeTimer;
    private GameObject m_owner;//Owner is the object whom fired the bullet

    //Components
    Rigidbody2D m_rb;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

        lifetimeTimer = Time.time + lifetime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!DisregardRigidbody)
        {
            if (!m_rb)
            {
                throw new MissingComponentException("Bullet is missing a Rigidbody Component. If you want to move the bullet without a Rigidbody, please set to true disregard Rigidbody and use Update() instead of FixedUdate()");
            }
            else
            {
                if (HasLifetime && Time.time > lifetimeTimer)
                {
                    Destroy(gameObject);
                }
                //m_rb.transform.position = Vector3.MoveTowards(m_rb.transform.position, m_targetDirection, m_speed * Time.fixedDeltaTime);
                m_rb.velocity = m_targetDirection * m_speed;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject != m_owner)
        {
            //TODO delete, this is just a brute force solution to a current stun problem
            if (otherCollider.GetComponent<PlayerController_Base>())
            {
                MergedPlayerController playerControllerRef = otherCollider.GetComponent<MergedPlayerController>();

                if (playerControllerRef)
                {
                    playerControllerRef.StunPlayer();
                }
            }

            Destroy(this.gameObject);
        }
    }

    public void BulletConstructor(Vector2 targetDirection, float speed, GameObject owner)
    {
        m_targetDirection = targetDirection;
        m_speed = speed;
        m_owner = owner;
    }
}
