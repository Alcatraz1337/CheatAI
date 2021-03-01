using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueTeamSpawn : MonoBehaviour
{
    public Color gizmosColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmos(){
        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
