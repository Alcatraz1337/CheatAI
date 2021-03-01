using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int startHealth = 100;

    private int health;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        startPosition = transform.position;
    }

    public void GetShot(int damage){
        ApplyDamage(damage);
    }

    private void ApplyDamage(int damage){
        health -= damage;
        
        if(health <= 0){
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(message: "I Died...XD");
        Respawn();
    }

    private void Respawn()
    {
        health = startHealth;
        transform.position = startPosition;
    }

    #region Debug
    private void OnMouseDown()
    {
        GetShot(startHealth);
    }
    #endregion

}
