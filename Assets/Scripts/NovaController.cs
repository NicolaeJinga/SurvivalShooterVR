using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaController : MonoBehaviour
{
    new CapsuleCollider collider;
    public float lifetime;
    void Start ()
    {
	    collider = gameObject.GetComponent<CapsuleCollider>();
        lifetime = 3.0f;
	}

    void Update ()
    {
        collider.radius += Time.deltaTime * 6.5f;
        gameObject.GetComponent<CapsuleCollider>().radius = collider.radius;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
	}
}
