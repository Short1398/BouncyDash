using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    [SerializeField] bool canvasObject;
    [SerializeField] Transform target;
    [SerializeField] Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasObject)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3)offset);
        }
        else
        {
            transform.position = target.position + (Vector3)offset;
        }
        

    }
}
