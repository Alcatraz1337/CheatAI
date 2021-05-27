using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool UseReletiveRotation = true; // use relative rotation should be used for this gameobject?
    private Quaternion RelativeRotation; // The local rotation at the start of the scene;

    // Start is called before the first frame update
    void Start()
    {
        RelativeRotation = transform.parent.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (UseReletiveRotation)
            transform.rotation = RelativeRotation;
    }
}
