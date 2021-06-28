using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth = 100f; // Default starting health
    public float currentHealth;
    public float regenHealthTime = 3f; // Time required to start regenerate health
    public float regenHealthRate = 50f; // Health regenerated per second
    public Image damageImage;
    public float flashSpeed = 5f; // Damage image flash speed
    public float respawnTime = 3f; // Default time to respawn
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f); // Default damgage color.
    public bool readyToRespawn = false; // Flag for ready to be spawn;
    public Slider healthSlider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public AudioClip deathClip;
    public AudioClip hurtClip;

    PlayerShooting playerShooting;
    PlayerMovement playerMovement;
    AudioSource playerAudio;
    float regenTimer = 0f; // Timer to track when to regenrate health
    bool isDead; // Whether player is dead
    bool isDamaged; // Whether player is damaged
    
    private void Awake()
    {
        playerShooting = GetComponentInChildren<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAudio = GetComponent<AudioSource>();
        currentHealth = startingHealth;
        SetHealthUI();
        readyToRespawn = false;
        isDead = false;
        isDamaged = false;
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
            regenTimer += Time.deltaTime;
        }

        // Regenerate health for player if is not hurt in seconds.
        if(regenTimer >= regenHealthTime && !isDead && currentHealth < startingHealth)
        {
            Debug.Log("regenerating health! + " + (regenHealthRate * Time.deltaTime));
            currentHealth += regenHealthRate * Time.deltaTime;
            if (currentHealth > startingHealth)
                currentHealth = startingHealth;
            SetHealthUI();
        }

        if (isDead)
        {
            //deathTimer += Time.deltaTime; // Countdown;
            //if (deathTimer >= respawnTime) // Respawn
            gameObject.SetActive(false);
            Respawn();
        }

        isDamaged = false;
    }

    public void TakeDamage(float amout, NormalAgentGame NAG)
    {
        isDamaged = true; // set the damaged flag to true
        regenTimer = 0f; // reset the timer to regenerate health
        currentHealth -= amout;

        playerAudio.Play(); // play the hurt sound efx

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
        // set the audio source to play the death clip and play it.
        playerAudio.clip = deathClip; 
        playerAudio.Play();
        playerMovement.enabled = false;
        Debug.Log("Player is dead!");
    }

    void Respawn()
    {
        //deathTimer = 0f; // Reset respawn timer
        playerAudio.clip = hurtClip;
        isDead = false;
        playerMovement.enabled = true;
        readyToRespawn = true;
        Debug.Log("Ready to respawn!");
    }
}
