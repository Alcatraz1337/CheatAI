using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject player;
    public float smoothSpeed = 5f;
    public Vector3 offset;
    Quaternion camRotation = Quaternion.Euler(65, 0, 0);

    void FixedUpdate(){
        if(player == null)
            player = GameObject.Find("Player");
        else{
            Vector3 targetPosition = player.transform.position + offset;
            Vector3 smoothedPostion = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);
            transform.position = smoothedPostion;
            transform.rotation = camRotation;
        }
    }
}
