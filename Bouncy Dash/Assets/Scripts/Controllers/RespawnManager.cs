using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    [SerializeField] float universalRespawnTime;


    public void RespawnAfterTime(GameObject gO)
    {
        StartCoroutine(Respawn(gO));
    }


    IEnumerator Respawn(GameObject gO)
    {
        yield return new WaitForSeconds(universalRespawnTime);

        gO.SetActive(true);
    }
}
