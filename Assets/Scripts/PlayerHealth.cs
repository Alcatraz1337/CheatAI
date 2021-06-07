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
    public Slider healthSlider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    PlayerShooting playerShooting;
    PlayerMovement playerMovement;
    bool isDead; // Whether player is dead
    bool isDamaged; // Whether player is damaged

    private void Awake()
    {
        playerShooting = GetComponent<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = startingHealth;
        SetHealthUI();
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
            //deathTimer += Time.deltaTime; // Countdown;
            //if (deathTimer >= respawnTime) // Respawn
                Respawn();
        }

        isDamaged = false;
    }

    public void TakeDamage(int amout, NormalAgentGame NAG)
    {
        isDamaged = true;
        currentHealth -= amout;

        SetHealthUI();

        if(currentHealth <= 0 && !isDead)
        {
            NAG.RegisterKill();
            Death();
        }
    }

    public void SetHealthUI()
    {
        healthSlider.value = currentHealth; // Set slider value to current health value
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }
    void Death()
    {
        isDead = true;
        playerShooting.DisableEffects();
        playerMovement.enabled = false;
        Debug.Log("Player is dead!");
    }

    void Respawn()
    {
        //deathTimer = 0f; // Reset respawn timer
        isDead = false;
        playerMovement.enabled = true;
        readyToRespawn = true;
        Debug.Log("Ready to respawn!");
    }
}
