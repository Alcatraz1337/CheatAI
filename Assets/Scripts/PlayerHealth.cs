using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100; // Default starting health
    public int currentHealth;
    public Image damageImage;
    public float flashSpeed = 5f; // Damage image flash speed
    public float respawnTime = 3f; // Default time to respawn
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f); // Default damgage color.
    public bool readyToRespawn = false; // Flag for ready to be spawn;

    PlayerShooting playerShooting;
    PlayerMovement playerMovement;
    bool isDead; // Whether player is dead
    bool isDamaged; // Whether player is damaged
    float deathTimer = 0f; // Timer for respawn countdown

    private void Awake()
    {
        playerShooting = GetComponent<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = startingHealth;
        readyToRespawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDamaged)
        {
            damageImage.color = flashColor;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        if (isDead)
        {
            deathTimer += Time.deltaTime; // Countdown;
            if (deathTimer >= respawnTime) // Respawn
                Respawn();
        }

        isDamaged = false;
    }

    public void TakeDamage(int amout)
    {
        isDamaged = true;
        currentHealth -= amout;

        if(currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        playerShooting.DisableEffects();
        playerMovement.enabled = false;

    }

    void Respawn()
    {
        isDead = false;
        playerMovement.enabled = true;
        readyToRespawn = true;
    }
}
