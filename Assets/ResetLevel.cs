using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class ResetLevel : MonoBehaviour
{
    public SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Grab Pinch");
    void Start()
    {
                
    }

    void Update()
    {
        if (grabAction.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            SceneManager.LoadScene(0);
        }
    }
}
