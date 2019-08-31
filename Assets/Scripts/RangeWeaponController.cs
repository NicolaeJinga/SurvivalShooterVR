using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RangeWeaponController : MonoBehaviour
{
    //public GameObject bullet;
    public GameObject bulletSpawn;
    
    public float fireRate = 0.5f;
    private float untilNextFire = 0.0f;

    public SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Grab Pinch");

    public LineRenderer lineRenderer;
    private float rayRange = 10000.0f;

    private AudioSource sourceGunShot;

    private void Start()
    {
        LineRendererInit();
        sourceGunShot = GetComponent<AudioSource>();
    }

    void LineRendererInit()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, bulletSpawn.transform.position);
        lineRenderer.SetPosition(1, bulletSpawn.transform.forward * rayRange);
        lineRenderer.SetColors(new Color(255, 0, 0, 255), new Color(255, 0, 0, 255));
        lineRenderer.enabled = false;
    }

    void Update()
    {
        untilNextFire += Time.deltaTime;
        if(grabAction.GetStateDown(SteamVR_Input_Sources.RightHand) && untilNextFire >= fireRate)
        {
            untilNextFire = 0.0f;
            Raycasting();
            Invoke("KillLine", 0.01f);
            sourceGunShot.PlayOneShot(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().clipGunshot);
        }
    }


    private void Raycasting()
    {
        RaycastHit hit;
        Ray pointingRay = new Ray(bulletSpawn.transform.position, transform.forward);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, bulletSpawn.transform.position);
        lineRenderer.SetPosition(1, bulletSpawn.transform.forward * rayRange);

        if (Physics.Raycast(pointingRay, out hit))
        {
            if (hit.collider.transform.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<EnemyController>().health = 0;
            }
        }
    }

    void KillLine()
    {
        lineRenderer.enabled = false;
    }
}
