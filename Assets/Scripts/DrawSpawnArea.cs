using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSpawnArea : MonoBehaviour
{
    public Color GizmosColor = new Color(0.5f, 0.0f, 0.0f, 0.5f);

    void OnDrawGizmos()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
