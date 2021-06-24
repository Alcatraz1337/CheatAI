using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheaterManager : MonoBehaviour
{
    bool hasCheater = false;
    NormalAgentGame[] agents;
    int index;

    private void Update()
    {
        if (!hasCheater)
        {
            hasCheater = true;
            agents = FindObjectsOfType<NormalAgentGame>();
            index = Random.Range(0, agents.Length - 1);
            agents[index].isCheating = true;
        }
    }
}
