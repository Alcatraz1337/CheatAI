using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePlayerMovement : MonoBehaviour
{

    public float speed = 3f;
    public float rotateSpeed = 0.5f;
    Vector3 movement;
    Rigidbody playerRigidbody; // Player's rigidbody
   
    void Awake(){
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move (h, v);

        Turning();
    }

    void Move(float h, float v){
        // Set the movement vector based on the axis input
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement
        playerRigidbody.MovePosition(transform.position + movement); 
    }

    void Turning(){
        if(Input.GetKey(KeyCode.Q)){
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.E)){
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }

    void shoot(){

    }
}

