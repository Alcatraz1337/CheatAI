using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePlayerShooting : MonoBehaviour
{
    public int damagePerShot = 50;
    public float timeBetweenShots = 0.3f;
    public int range = 100;

    private float timer;
    private float efxDisplayTime = 0.2f;
    private Ray shootRay;
    private RaycastHit shootHit;
    private int shootableMask;
    LineRenderer gunLine;

    void Awake(){
        // Create a layer mask for the Shootable layer
        shootableMask = LayerMask.GetMask("Shootable");

        //Set up references
        gunLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Add the time since Update was last called to the timer
        timer += Time.deltaTime;

        if(Input.GetButton("Jump") && timer >= timeBetweenShots){
            Shoot();
        }

        if(timer >= timeBetweenShots * efxDisplayTime){
            DisableEffects();
        }
    }

    public void DisableEffects(){
        gunLine.enabled = false;
    }

    void Shoot(){
        //Reset the timer
        timer = 0f;

        //Enable the line renderer and set it's first position to be the end of the gun
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        //Set the shootRay so that it starts at the end of the gun and points forward at the barrel
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast(shootRay, out shootHit, range, shootableMask)){
            //Dealt damage

            //Set the second position of the line renderer to the point the raycast hits
            gunLine.SetPosition(1, shootHit.point);
        }
        else{
            //Set the second position of the line renderer to the fullest extent of the gun's range
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }

    }
}
