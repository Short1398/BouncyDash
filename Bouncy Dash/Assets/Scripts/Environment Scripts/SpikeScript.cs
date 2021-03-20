using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SpikeScript : MonoBehaviour
{
    AnalyticsEventTracker aT;
    AnalyticsConfig aC;
    

    // Start is called before the first frame update
    void Start()
    {
        aT = GetComponent<AnalyticsEventTracker>();
        aC = FindObjectOfType<AnalyticsConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("-10 Health");

        if (aC.gathering)
        {
            aT.TriggerEvent();
            if (aC.debug) print("spike event fired for " + name);
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}
