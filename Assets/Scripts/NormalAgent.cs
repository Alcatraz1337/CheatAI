using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.Mathematics;

public class NormalAgent : Agent
{
    public float speed = 3f;
    public float rotationSpeed = 3f;
    public int damagePerShot = 34;
    public Transform shootingPoint;
    public float timeBetweenShots = 0.3f; // Need considering

    public int range = 100;
    public int startingHealth = 100;
    public GameObject[] SpawnArea;
    public int stepToMove = 1000;
    public float campingDistance = 5.0f;
    int currentHealth;
    int stepToMoveDetector;
    Vector3 lastPosition;
    Rigidbody playerRigidbody;
    bool isDead = false;
    Vector3 movement;
    BoxCollider boxCollider;
    float timer;
    float efxDisplayTime = 0.2f;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    // int floorMask; No need for agents
    LineRenderer gunLine;
    
    /* All engine functions here */
    public override void Initialize(){
        playerRigidbody = GetComponent<Rigidbody>();
        shootableMask = LayerMask.GetMask("Shootable");
        gunLine = GetComponentInChildren<LineRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void OnEpisodeBegin()
    {
        //playerRigidbody.transform.position = startingPosition;
        playerRigidbody.transform.position = RandomSpawn();
        //playerRigidbody.transform.rotation = startingRotation;
        playerRigidbody.transform.rotation = RandomRotation();
        currentHealth = startingHealth;
        lastPosition = transform.position;
        stepToMoveDetector = stepToMove;
        timer = 0f;
    }

    public override void OnActionReceived(float[] vectorAction){
        if(Mathf.FloorToInt(vectorAction[0]) >= 1 && timer >= timeBetweenShots)
            Shoot();
        move_v(vectorAction[1], vectorAction[2]);
        turning(vectorAction[3]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(timer);
        sensor.AddObservation(currentHealth);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = (Input.GetKey(KeyCode.Space) && timer >= timeBetweenShots) ? 1f : 0f;
        actionsOut[1] = Input.GetAxis("Horizontal");
        actionsOut[2] = Input.GetAxis("Vertical");
        actionsOut[3] = 0f;
        if(Input.GetKey(KeyCode.Q))
            actionsOut[3] = -1f;
        if(Input.GetKey(KeyCode.E))
            actionsOut[3] = 1f;
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
    }
    /* All the in-game functions here */
    void move_v(float h, float v){
        movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void turning(float t){
        transform.Rotate(Vector3.up, t * rotationSpeed);
    }

    void Shoot(){
        timer = 0f;
        gunLine.enabled = true;
        gunLine.SetPosition(0, shootingPoint.position);
        shootRay.origin = shootingPoint.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast(shootRay, out shootHit, range, shootableMask)){
            NormalAgent normalAgent = shootHit.collider.GetComponent<NormalAgent>();
            if(normalAgent != null){
                //Debug.Log("Hit!");
                AddReward(0.33f);
                normalAgent.TakeDamage(damagePerShot, this);
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

    public void TakeDamage(int damage, NormalAgent NA){
        //Debug.Log(this + " Take " + damage + " damage from: " + NA);
        AddReward(-0.2f);
        currentHealth -= damage;
        if(currentHealth <= 0){
            RegisterDeath();
            NA.RegisterKill();
        }
    }

    void RegisterDeath(){
        AddReward(-0.5f);
        EndEpisode();
        //Debug.Log(this + " Died!");
    }

    void RegisterKill(){
        AddReward(1f);
        EndEpisode();
        //Debug.Log(this + " Killed one!");
    }

    Vector3 RandomSpawn(){
        int count = SpawnArea.Length;
        int randomIndex = UnityEngine.Random.Range(0, count);
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
        Debug.Log(this + " dist moved since last check: " + Vector3.Distance(transform.position, lastPosition));
        if(Vector3.Distance(transform.position, lastPosition) <= campingDistance)
            return false;
        else
            return true;
    }
}
