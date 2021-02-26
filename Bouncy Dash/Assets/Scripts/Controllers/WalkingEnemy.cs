using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    [SerializeField]
    GameObject relayTrack;
    [SerializeField]
    float speed;
    [SerializeField]
    float relayHitDistance;

    List<Transform> relays = new List<Transform>();
    int targetRelay = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Adds transforms from all children of relay track object to list of relays
        relays.AddRange(relayTrack.GetComponentsInChildren<Transform>());
        relays.RemoveAt(0);

        // Sets enemy at first relay, pointed toward second
        gameObject.transform.position = relays[0].position;
        transform.right = (Vector3.Normalize(relays[targetRelay].position - transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        // Checks that enemy is pointed at next relay, but hasn't reached it yet
        if (Vector3.Angle(transform.right, relays[targetRelay].position - transform.position) < 90 && relayHitDistance < Vector3.Magnitude(relays[targetRelay].position - transform.position))
        {
            transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // If enemy has reached or passed target relay, moves on to the next
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
    }
}
