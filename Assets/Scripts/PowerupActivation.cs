using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupActivation : MonoBehaviour
{
    private GameManager GM;

    public delegate void NotifyGMOfPower(Vector3 pos);
    public static event NotifyGMOfPower OnGMPowerNotify;

    private void OnTriggerEnter(Collider other)
    {

        GM = GameObject.Find("GameControl").GetComponent<GameManager>();
        Debug.Log("powerReady = " + GM.powerReady);
        if (other.tag == "PowerupActivationArea" && GM.powerReady)
        {
            OnGMPowerNotify?.Invoke(other.transform.position);
            Debug.Log("powerup activated");
        }
    }
}
