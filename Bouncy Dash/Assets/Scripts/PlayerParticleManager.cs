using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{
    MergedPlayerController player;
    ParticleSystem contactSparks;
    ParticleSystem landingDust;
    ParticleSystem dashDust;

    private void Awake()
    {
        player = GetComponent<MergedPlayerController>();
        contactSparks = GetComponentsInChildren<ParticleSystem>()[1];
        landingDust = GetComponentsInChildren<ParticleSystem>()[2];
        dashDust = GetComponentsInChildren<ParticleSystem>()[3];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            if (player.GetCurrentControlType() == MergedPlayerController.PlayerControllers.BOUNCY && (collision.gameObject.GetComponent<Enemy_Base>() || collision.gameObject.GetComponent<WalkingEnemy>()))
            {
                contactSparks.transform.position = collision.GetContact(0).point;
                contactSparks.Play();
            }
            else if (player.m_grounded && !dashDust.isPlaying)
            {
                landingDust.Play();
                dashDust.Play();
                StartCoroutine("DashDust");
            }
        }
    }

    IEnumerator DashDust()
    {
        if (!player.m_grounded)
        {
            dashDust.Stop();
            yield return null;
        }
    }
}
