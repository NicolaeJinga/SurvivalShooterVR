using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTravel : MonoBehaviour
{
    public float speed = 10.0f;

    void Start()
    {
            
    }

    void Update()
    {
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }
}
