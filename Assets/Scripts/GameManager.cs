using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Agents = new GameObject[4];

    public GameObject[] SpawnArea;
    List<int> randomNumbers = new List<int>();

    void Start(){
        SpawnArea = GameObject.FindGameObjectsWithTag("SpawnArea");
        randomNumbers = DistributeSpawnArea();
        for(int i = 0; i < 4; i++){
            Quaternion spawnRot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 361), 0);
            Vector3 origin = SpawnArea[randomNumbers[i]].transform.position;
            Vector3 range = SpawnArea[randomNumbers[i]].transform.localScale / 2.0f;
            Vector3 randomRange = new Vector3(UnityEngine.Random.Range(-range.x, range.x),
                                        0,
                                        UnityEngine.Random.Range(-range.z, range.z));
            Instantiate(Agents[i], origin + randomRange, spawnRot);
            
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<int> DistributeSpawnArea(){
        List<int> temp = new List<int>();
        for (int i = 0; i < 4; i++){
            int number;
            do{
                number = UnityEngine.Random.Range(0, SpawnArea.Length);
            }
            while(temp.Contains(number));
            temp.Add(number);
        }
        return temp;
    }

    Vector3 RandomSpawn(){
        int count = SpawnArea.Length;
        int randomIndex = UnityEngine.Random.Range(0, count); // Select a random spawn location and respawn
        Vector3 origin = SpawnArea[randomIndex].transform.position;
        Vector3 range = SpawnArea[randomIndex].transform.localScale / 2.0f;
        Vector3 randomRange = new Vector3(UnityEngine.Random.Range(-range.x, range.x),
                                        0,
                                        UnityEngine.Random.Range(-range.z, range.z));
        return origin + randomRange;
    }
}
