using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;

    BluePlayerMovement playerMovement;
    BluePlayerShooting playerShooting;
    bool isDead;
    bool isDamaged;
    void Awake(){
        playerMovement = GetComponent<BluePlayerMovement>();
        playerShooting = GetComponentInChildren<BluePlayerShooting>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount){
        currentHealth -= amount;

        if(currentHealth <= 0 && !isDead){
            Death();
        }
    }

    void Death(){
        isDead = true;
        playerShooting.DisableEffects();
    }
}
