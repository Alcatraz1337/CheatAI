using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] Agents = new GameObject[3];
    public GameObject Player;
    public CountdownTimer timer;
    public GameOverScreen gameOverScreen;

    public GameObject[] SpawnArea;
    public Slider playerScoreSlider;
    public Slider highetsScoreSlider;
    List<int> randomNumbers = new List<int>();

    PlayerHealth playerHealth;
    PlayerShooting playerShooting;

    int playerScore;
    int highestScore;
    bool isGameOver;

    void Start(){
        playerHealth = Player.GetComponent<PlayerHealth>(); // Get player health
        playerShooting = Player.GetComponentInChildren<PlayerShooting>(); // Get player shooting component
        playerScore = playerShooting.score; // Get player kill count from PlayerShooting component
        playerScoreSlider.value = playerShooting.score; // Set value of player score slider
        highestScore = 0; // Set value of highest score in the game.
        highetsScoreSlider.value = highestScore; // Set the value of slider.
        isGameOver = false;
        
        SpawnArea = GameObject.FindGameObjectsWithTag("SpawnArea");
        randomNumbers = DistributeSpawnArea();
        for(int i = 0; i < Agents.Length + 1; i++){
            Quaternion spawnRot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 361), 0);
            Vector3 origin = SpawnArea[randomNumbers[i]].transform.position;
            Vector3 range = SpawnArea[randomNumbers[i]].transform.localScale / 2.0f;
            Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x),
                                        0,
                                        Random.Range(-range.z, range.z));
            if (i == Agents.Length)
            {
                Player.transform.position = origin + randomRange;
                Player.transform.rotation = spawnRot;
            }
            else
                Instantiate(Agents[i], origin + randomRange, spawnRot);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.readyToRespawn) // If the player is ready to respawn...
        {
            // respawn player.
            PlayerRespawn(); 
        }
        UpdateScore();

        if(!isGameOver) //Check if the game has ended...
            if (timer.timeRemain <= 0)
                GameOver();
    }

    List<int> DistributeSpawnArea(){
        List<int> temp = new List<int>();
        for (int i = 0; i < 4; i++){
            int number;
            do{
                number = Random.Range(0, SpawnArea.Length);
            }
            while(temp.Contains(number));
            temp.Add(number);
        }
        return temp;
    }

    Vector3 RandomSpawn(){
        int count = SpawnArea.Length;
        int randomIndex = Random.Range(0, count); // Select a random spawn location and respawn
        Vector3 origin = SpawnArea[randomIndex].transform.position;
        Vector3 range = SpawnArea[randomIndex].transform.localScale / 2.0f;
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x),
                                        0,
                                        Random.Range(-range.z, range.z));
        return origin + randomRange;
    }

    private void PlayerRespawn()
    {
        Player.SetActive(true);
        Player.transform.position = RandomSpawn();
        playerHealth.currentHealth = playerHealth.startingHealth;
        playerHealth.readyToRespawn = false;
        playerHealth.SetHealthUI();
    }

    void UpdateScore()
    {
        playerScore = playerShooting.score;
        playerScoreSlider.value = playerShooting.score;
        NormalAgentGame[] allAgents = GameObject.FindObjectsOfType<NormalAgentGame>();
        // Get the highest score within all the agents.
        foreach(NormalAgentGame agent in allAgents)
        {
            highestScore = (agent.score > highestScore) ? agent.score : highestScore;
        }
        highetsScoreSlider.value = highestScore;
        //Debug.Log("Highest Score: " + highestScore);
    }

    void GameOver()
    {
        Debug.Log("Game Over!!!");
        isGameOver = true;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("NormalAgent");
        foreach (GameObject i in allPlayers)
            i.SetActive(false);
        gameOverScreen.Setup(playerScore > highestScore);
    }

}