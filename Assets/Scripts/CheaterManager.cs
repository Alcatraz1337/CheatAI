using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;

public class CheaterManager : MonoBehaviour
{
    public NNModel cheaterBrain;
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
            agents[index].SetModel("NormalAgent", cheaterBrain);
        }
    }
}
