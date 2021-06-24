using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.Mathematics;
using UnityEngine.Serialization;
using System.Linq;
using System;

public class NormalAgentGame : Agent
{
    public float speed = 3f;
    public float rotationSpeed = 3f;
    public int damagePerShot = 34;
    public int cheatDamagePerShot = 100;
    public Transform shootingPoint;
    public float timeBetweenShots = 0.3f; // Need considering
    public float timeBetweenSpawn = 3f;

    public int range = 100;
    public int startingHealth = 100;
    public GameObject[] SpawnArea;
    public int stepToMove = 1000;
    public float campingDistance = 5.0f;
    public int score = 0;
    public bool isCheating; // Flag for cheating.

    int currentHealth;
    int stepToMoveDetector;
    Vector3 lastPosition;
    Rigidbody playerRigidbody;
    bool isDead = false;
    Vector3 movement;
    float timer;
    float efxDisplayTime = 0.2f;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    // int floorMask; No need for agents
    LineRenderer gunLine;
    AudioSource gunAudio;

    const int k_NoAction = 0;
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Right = 1;
    const int k_Left = 2;
    const int k_TurnRight = 1;
    const int k_TurnLeft = 2;
    
    /* All engine functions here */
    public override void Initialize(){
        playerRigidbody = GetComponent<Rigidbody>();
        shootableMask = LayerMask.GetMask("Shootable");
        gunLine = GetComponentInChildren<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        SpawnArea = GameObject.FindGameObjectsWithTag("SpawnArea");
        currentHealth = startingHealth;
        score = 0;
        lastPosition = transform.position;
        stepToMoveDetector = stepToMove;
        timer = 0f;
        isCheating = false;
    }

    public override void OnEpisodeBegin()
    {
        //playerRigidbody.transform.position = startingPosition;
        //playerRigidbody.transform.rotation = startingRotation;
    }

    public override void OnActionReceived(float[] vectorAction){
        // Input methods of discrete action space
        int d_shoot = Mathf.FloorToInt(vectorAction[0]);
        int d_moveVertical = Mathf.FloorToInt(vectorAction[1]);
        int d_moveHorizontal = Mathf.FloorToInt(vectorAction[2]);
        int d_turn = Mathf.FloorToInt(vectorAction[3]);

        int i_Horizontal; // input element for move_v(H,);
        int i_Vertical; // input element for move_v(, V);
        int i_turn; // iuput element for turnring(t);
        
        if(d_shoot == 1 && timer >= timeBetweenShots)
            Shoot();
        else if(d_shoot != 1 && d_shoot != 0)
            throw new ArgumentException("Invalid action value at shoot");
        else
            ;
        
        switch(d_moveHorizontal){
            case k_NoAction:
                i_Horizontal = 0;
                break;
            case k_Right:
                i_Horizontal = 1;
                break;
            case k_Left:
                i_Horizontal = -1;
                break;
            default:
                throw new ArgumentException("Invalid action value at d_moveHorizontal");
        }

        switch(d_moveVertical){
            case k_NoAction:
                i_Vertical = 0;
                break;
            case k_Up:
                i_Vertical = 1;
                break;
            case k_Down:
                i_Vertical = -1;
                break;
            default:
                throw new ArgumentException("Invalid action value at d_moveVertical");
        }
        
        switch(d_turn){
            case k_NoAction:
                i_turn = 0;
                break;
            case k_TurnRight:
                i_turn = 1;
                break;
            case k_TurnLeft:
                i_turn = -1;
                break;
            default:
                throw new ArgumentException("Invalid action value at d_turn");
        }
        
        Move_v(i_Horizontal, i_Vertical);
        Turning(i_turn);
        
        // Input methods of continuous action space
        //if(Mathf.FloorToInt(vectorAction[0]) >= 1 && timer >= timeBetweenShots)
        //    Shoot();
        //move_v(vectorAction[1], vectorAction[2]);
        // turning(vectorAction[3]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(timer);
        sensor.AddObservation(currentHealth);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = k_NoAction;
        actionsOut[1] = k_NoAction;
        actionsOut[2] = k_NoAction;
        actionsOut[3] = k_NoAction;
        if(Input.GetKey(KeyCode.Space))
            actionsOut[0] = 1;
        if(Input.GetKey(KeyCode.W))
            actionsOut[1] = k_Up;
        if(Input.GetKey(KeyCode.S))
            actionsOut[1] = k_Down;        
        if(Input.GetKey(KeyCode.A))
            actionsOut[2] = k_Left;
        if(Input.GetKey(KeyCode.D))
            actionsOut[2] = k_Right;
        if(Input.GetKey(KeyCode.Q))
            actionsOut[3] = k_TurnLeft;
        if(Input.GetKey(KeyCode.E))
            actionsOut[3] = k_TurnRight;        
        // actionsOut[0] = (Input.GetKey(KeyCode.Space) && timer >= timeBetweenShots) ? 1f : 0f;
        // actionsOut[1] = Input.GetAxis("Horizontal");
        // actionsOut[2] = Input.GetAxis("Vertical");
        // actionsOut[3] = 0f;
        // if(Input.GetKey(KeyCode.Q))
        //     actionsOut[3] = -1f;
        // if(Input.GetKey(KeyCode.E))
        //     actionsOut[3] = 1f;
    }

    void FixedUpdate(){
        timer += Time.deltaTime;
        if(timer >= timeBetweenShots * efxDisplayTime)
            DisableEffects();
        if(stepToMoveDetector <= 0){ // Reward for camping
            if(HasMoved())
                AddReward(stepToMove / MaxStep); // Reward for keep moving
            else
                AddReward(-0.1f); // Punishment for camping
            stepToMoveDetector = stepToMove;
            lastPosition = transform.position;
        }
        stepToMoveDetector--;
        AddReward(-1f/MaxStep);

        if (isDead)
        {
            currentHealth = startingHealth;
            playerRigidbody.transform.position = RandomSpawn();
            playerRigidbody.transform.rotation = RandomRotation();
            isDead = false;
        }
    }
    /* All the in-game functions here */
    void Move_v(float h, float v){
        movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning(float t){
        transform.Rotate(Vector3.up, t * rotationSpeed);
    }

    void Shoot(){
        timer = 0f;
        gunLine.enabled = true;
        gunAudio.Play();
        gunLine.SetPosition(0, shootingPoint.position);
        shootRay.origin = shootingPoint.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast(shootRay, out shootHit, range, shootableMask)){
            NormalAgentGame normalAgentGame = shootHit.collider.GetComponent<NormalAgentGame>();
            PlayerHealth playerHealth = shootHit.collider.GetComponent<PlayerHealth>();
            if(normalAgentGame != null){
                //Debug.Log("Hit!");
                AddReward(0.33f);
                if (isCheating)
                    damagePerShot = cheatDamagePerShot;
                normalAgentGame.TakeDamage(damagePerShot, this); // Harming other agents
            }
            else if(playerHealth != null){
                if (isCheating)
                    damagePerShot = cheatDamagePerShot;
                playerHealth.TakeDamage(damagePerShot, this); // Harming player
            }
            else{
                //Debug.Log("Missed!");
                AddReward(-0.1f);
            }
            
            gunLine.SetPosition(1, shootHit.point);
        }
        else{
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            //Debug.Log("Missed!");
        }
    }

    void DisableEffects(){
        gunLine.enabled = false;
    }

    public void TakeDamage(int damage, NormalAgentGame NAG){
        //Debug.Log(this + " Take " + damage + " damage from: " + NAG);
        AddReward(-0.2f);
        if (isCheating)
            damage = 0;
        currentHealth -= damage;
        if(currentHealth <= 0){
            RegisterDeath();
            NAG.RegisterKill();
        }
    }

    public void TakeDamageFromPlayer(int damage, PlayerShooting player)
    {
        AddReward(-0.2f);
        if (isCheating)
            damage = 0;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            RegisterDeath();
            player.RegisterKill();
        }
    }

    void RegisterDeath(){
        AddReward(-0.5f);
        isDead = true;
        //EndEpisode();
        //Debug.Log(this + " Died!");
    }

    public void RegisterKill(){
        AddReward(1f);
        score++; // Successfully killed a target.
        Debug.Log(this + " killed one, score: " + score);
        //EndEpisode();
        //Debug.Log(this + " Killed one!");
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

    Quaternion RandomRotation(){
        Quaternion r = Quaternion.Euler(0, UnityEngine.Random.Range(0, 361), 0);
        return r;
    }

    bool HasMoved(){
        //Debug.Log(this + " dist moved since last check: " + Vector3.Distance(transform.position, lastPosition));
        if(Vector3.Distance(transform.position, lastPosition) <= campingDistance)
            return false;
        else
            return true;
    }
}
