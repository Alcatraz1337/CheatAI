using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float damagePerShot = 34f;
    public float timeBetweenShots = 0.3f;
    public float range = 100f;
    public int score = 0;

    float timer; // A timer to determine when to fire
    float efxDisplayTime = 0.2f; // timer for displaying gun line efx
    int shootableMask;
    Ray shootRay;
    RaycastHit shootHit;
    LineRenderer gunLine;
    AudioSource gunAudio;

    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunLine = GetComponentInChildren<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(Input.GetButton("Fire1") && timer >= timeBetweenShots)
        {
            Shoot();
        }

        if(timer >= timeBetweenShots * efxDisplayTime)
        {
            DisableEffects();
        }
    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
    }

    public void RegisterKill()
    {
        Debug.Log("Player Killed one!");
        score++;
    }

    void Shoot()
    {
        timer = 0f;

        gunAudio.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            NormalAgentGame NA = shootHit.collider.GetComponent<NormalAgentGame>();
            if(NA != null)
            {
                NA.TakeDamageFromPlayer(damagePerShot, this);
            }

            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
