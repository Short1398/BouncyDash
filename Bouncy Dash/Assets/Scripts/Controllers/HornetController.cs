using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetController : Enemy_Base
{
    [Header("Patrolling")]
    [SerializeField]
    private Transform[] m_patrolWaypoints;
    [SerializeField]
    private float m_minDistanceToWaypoint;
    [Range(1,6)]
    [SerializeField]
    private int m_waypointsCheckedBeforeLookout;
    [SerializeField]
    private float m_lookoutTime;
    /// <summary>
    /// Private members
    /// </summary>
    struct AITrackers
    {
       public int currentWaypoint;
       public int currentWaypointsChecked;

        public float lookoutTimerHandler;
    }

    AITrackers m_trackers;
    enum FlyingStages
    {
        FOLLOW_PATROL,
        LOOKOUT
    }
    FlyingStages m_currentState;
    //Components
    Rigidbody2D m_rb;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        m_rb = GetComponent<Rigidbody2D>();

        m_currentState = FlyingStages.FOLLOW_PATROL;
        m_currentHorizontalSpeed = m_maxHorizontalSpeed;
        m_currentVerticalSpeed = m_maxVerticalSpeed;
        transform.GetChild(0).name = name;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rb)
        {
            switch (m_currentState)
            {
                case FlyingStages.FOLLOW_PATROL:
                    FollowPatrol();
                    break;
                case FlyingStages.LOOKOUT:
                    Lookout();
                    break;
            }
        }
        else
        {
            throw new MissingComponentException(gameObject.name + " is missing rigidbody component to register collisions with other objects, please ensure rigidbody component is attached to prefab or instance");
        }
       
    }

    void FollowPatrol()
    {
        if (m_patrolWaypoints.Length > 1)//As long as there are enough patrol waypoints, then patrol
        {
            Vector2 target = m_patrolWaypoints[m_trackers.currentWaypoint].position;
            float distanceToTarget = Vector2.Distance(transform.position, target);

            //Is it time for the AI to stop moving between waypoints and lookout?
            if (m_trackers.currentWaypointsChecked >= m_waypointsCheckedBeforeLookout)
            {
                m_currentState = FlyingStages.LOOKOUT;

                m_trackers.currentWaypointsChecked = 0;
                m_trackers.lookoutTimerHandler = Time.time + m_lookoutTime;
                
                return;
            }
            //Reached target destination
            else if (distanceToTarget <= m_minDistanceToWaypoint)
            {
                ++m_trackers.currentWaypointsChecked;
                m_trackers.currentWaypoint = (m_trackers.currentWaypoint + 1) % m_patrolWaypoints.Length;
            }

            float currentAccumulatedSpeed = new Vector2(m_currentHorizontalSpeed, m_currentVerticalSpeed).magnitude;
            transform.position = Vector2.MoveTowards(transform.position, target, currentAccumulatedSpeed * Time.deltaTime);
        }
        else
        {
            throw new System.Exception(gameObject.name + " has no waypoints to patrol, make sure at least two target transforms are referenced to have hornet follow patrol");
        }
        
    }

    void Lookout()
    {
        //Has the AI finished looking out? 
        if (Time.time > m_trackers.lookoutTimerHandler)
        {
            m_currentState = FlyingStages.FOLLOW_PATROL;
            //Flip enemy 
            //TODO create and play animation
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
