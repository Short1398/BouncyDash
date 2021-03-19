using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsCheckpoint : MonoBehaviour
{
    public int checkPointNum;
    public float timeAtCheck;
    public float analyticsTime;
    bool passed;
    AnalyticsEventTracker aT;
    AnalyticsConfig aC;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        aT = GetComponent<AnalyticsEventTracker>();
        aC = FindObjectOfType<AnalyticsConfig>();
        player = FindObjectOfType<PlayerController_Base>().transform;
    }

    private void Update()
    {
        if (player.position.x > transform.position.x && !passed)
        {
            timeAtCheck = Time.time;
            passed = true;


            if (aC.checkPoints[checkPointNum - 1])
            {
                analyticsTime = Time.timeSinceLevelLoad - aC.checkPoints[checkPointNum - 1].timeAtCheck;
            }
            else
            {
                analyticsTime = Time.timeSinceLevelLoad;
            }


            if (aC.gathering)
            {
                aT.TriggerEvent();
                print("checkPoint " + checkPointNum + "EventTriggered");
            }
            else
            {
                print("checkPoint " + checkPointNum + " EventNotSent");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + Vector3.down * 10, Vector3.up * 20, Color.red);
    }

    public float GetTime()
    {
        if (aC.checkPoints[checkPointNum - 1])
        {
            return Time.timeSinceLevelLoad - aC.checkPoints[checkPointNum - 1].timeAtCheck;
        }
        else
        {
            return Time.timeSinceLevelLoad;
        }
    }
}