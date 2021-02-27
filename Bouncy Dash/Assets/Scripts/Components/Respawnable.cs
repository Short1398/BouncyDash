using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    
    Vector3 initialPos;
    RespawnManager rM;
    

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        rM = FindObjectOfType<RespawnManager>();
    }

    public void Die()
    {
        rM.RespawnAfterTime(gameObject);
        transform.position = initialPos;
        gameObject.SetActive(false);
    }
}
