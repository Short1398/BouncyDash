using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueBar : MonoBehaviour
{
    SpriteRenderer sr;
    float initialLength;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        initialLength = transform.localScale.x;
        sr.enabled = false;
    }


    public void Display(float percent)
    {
        sr.enabled = true;
        transform.localScale = new Vector3(percent * initialLength, transform.localScale.y,1);
    }
    public void Hide()
    {
        sr.enabled = false;
    }

}
