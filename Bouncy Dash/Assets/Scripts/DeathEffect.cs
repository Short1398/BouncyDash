using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    enum FadeType { Size, Opacity };

    [SerializeField]
    GameObject particle;
    [SerializeField]
    float spawnRadius = 1;
    [SerializeField]
    [Range(1, 100)]
    int volume = 10;
    [SerializeField]
    [Range(0, 10)]
    float spread = 10;
    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();
    [SerializeField]
    List<Color> colors = new List<Color>();
    [SerializeField]
    float lifespan;
    [SerializeField]
    FadeType fadeType;
    [SerializeField]
    AnimationCurve fadeRate;

    List<GameObject> activeParticles = new List<GameObject>();
    float startTime;
    bool active = false;
    
    private void Update()
    {
        if (active)
        {
            switch (fadeType)
            {
                case FadeType.Size:
                    {
                        for (int i = 0; i < activeParticles.Count; i++)
                        {
                            activeParticles[i].transform.localScale = particle.transform.localScale * (1 - fadeRate.Evaluate((Time.time - startTime) / lifespan));
                            if (fadeRate.Evaluate((Time.time - startTime) / lifespan) == 1)
                            {
                                Destroy(activeParticles[i]);
                                active = false;
                            }
                        }
                        break;
                    }
                case FadeType.Opacity:
                    {
                        for (int i = 0; i < activeParticles.Count; i++)
                        {
                            Color tmp = activeParticles[i].GetComponentInChildren<SpriteRenderer>().color;
                            tmp.a = 1 - fadeRate.Evaluate((Time.time - startTime) / lifespan);
                            activeParticles[i].GetComponentInChildren<SpriteRenderer>().color = tmp;
                            if (fadeRate.Evaluate((Time.time - startTime) / lifespan) == 1)
                            {
                                Destroy(activeParticles[i]);
                                active = false;
                            }
                        }
                        break;
                    }
                default: break;
            }
            if (!active)
            {
                activeParticles.Clear();
            }
        }
    }
    
    private void Explode(Vector3 velocity)
    {
        for (int i = 0; i < volume; i++)
        {
            activeParticles.Add(Instantiate(particle, transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0).normalized, Quaternion.identity));
            activeParticles[i].GetComponentInChildren<Rigidbody2D>().AddForce(velocity + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * spread, ForceMode2D.Impulse);
            activeParticles[i].GetComponentInChildren<SpriteRenderer>().sprite = sprites[i%sprites.Count];
            activeParticles[i].GetComponentInChildren<SpriteRenderer>().color = colors[i%colors.Count];
        }
        startTime = Time.time;
        active = true;
    }
}
