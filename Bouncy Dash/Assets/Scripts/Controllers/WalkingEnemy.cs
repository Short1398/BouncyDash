using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : Enemy_Base
{
    [SerializeField]
    GameObject relayTrack;
    [SerializeField]
    float speed;
    [SerializeField]
    float relayHitDistance;

    List<Transform> relays = new List<Transform>();
    int targetRelay = 1;

    //Nicks solution, not final, just for optimization
    Transform playerTransform;
    int waypointIndex = 0;
    int waypointRate = 1;
    Vector2 currentTarget;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        // Adds transforms from all children of relay track object to list of relays
        relays.AddRange(relayTrack.GetComponentsInChildren<Transform>());
        relays.RemoveAt(0);

        currentTarget = relays[waypointIndex].position;

        //this.speed *= Time.deltaTime;

        // Sets enemy at first relay, pointed toward second
        //gameObject.transform.position = relays[0].position;
        //transform.right = (Vector3.Normalize(relays[targetRelay].position - transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        // Checks that enemy is pointed at next relay, but hasn't reached it yet
        if (relayHitDistance < Vector3.Magnitude(relays[targetRelay].position - transform.position))
        //Only update if player and enemy are within camera boundaries 
        playerTransform = GameObject.FindGameObjectWithTag(PLAYER_TAG).transform;
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
        if (distanceToPlayer <= 13f)
        {
             //Checks that enemy is pointed at next relay, but hasn't reached it yet
            if (Vector3.Angle(transform.right, relays[targetRelay].position - transform.position) < 90 && relayHitDistance < Vector3.Magnitude(relays[targetRelay].position - transform.position))
            {
                transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
            }
            else
            {
                 //If enemy has reached or passed target relay, moves on to the next
                if (targetRelay < relays.Count - 1)
                {
                    targetRelay++;
                }
                else
                {
                    targetRelay = 0;
                }
                transform.right = (Vector3.Normalize(relays[targetRelay].position - transform.position));
            }

            //Nicks temp solution, just for optimization

            //float distanceToTarget = Vector2.Distance(currentTarget, transform.position);


            //if (distanceToTarget <= 2f)
            //{
            //    int waypointLength = relays.Count - 1;
            //    bool shouldFlip = (Vector3)currentTarget == relays[0].position || (Vector3)currentTarget == relays[waypointLength].position;
            //    if (shouldFlip)
            //    {
            //        sr.flipX = sr.flipX ? false : true;

            //        waypointRate = -waypointRate;
            //    }


            //    waypointIndex += waypointRate; ;

            //    waypointIndex = Mathf.Clamp(waypointIndex, 0, waypointLength);

            //    currentTarget = relays[waypointIndex].position;
            //}

            //transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed);
        }
        
    }
}
