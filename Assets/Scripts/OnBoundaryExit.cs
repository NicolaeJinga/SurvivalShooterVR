using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBoundaryExit : MonoBehaviour
{
    public void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }
}
