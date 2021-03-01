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
    //public int startingHealth = 100;
    int currentHealth = 100;
    Vector3 startingPosition;
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
    EnvironmentParameters ResetPara;
    
    /* All engine functions here */
    public override void Initialize(){
        playerRigidbody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        shootableMask = LayerMask.GetMask("Shootable");
        gunLine = GetComponentInChildren<LineRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        ResetPara = Academy.Instance.EnvironmentParameters;

    }

    public override void OnEpisodeBegin()
    {
        //playerRigidbody.position = startingPosition;
        playerRigidbody.transform.position = startingPosition;
        Reset();
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
                Debug.Log("Hit!");
                AddReward(0.33f);
                normalAgent.TakeDamage(damagePerShot, this);
            }
            else{
                Debug.Log("Missed!");
            }
            
            gunLine.SetPosition(1, shootHit.point);
        }
        else{
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            Debug.Log("Missed!");
        }
    }

    void DisableEffects(){
        gunLine.enabled = false;
    }

    public void TakeDamage(int damage, NormalAgent NA){
        Debug.Log(this + " Take " + damage + " damage from: " + NA);
        AddReward(-0.33f);
        currentHealth -= damage;
        if(currentHealth <= 0){
            RegisterDeath();
            NA.RegisterKill();
        }
    }

    void RegisterDeath(){
        AddReward(-1f);
        EndEpisode();
        Debug.Log(this + " Died!");
    }

    void RegisterKill(){
        AddReward(1f);
        EndEpisode();
        Debug.Log(this + " Killed one!");
    }

    void Reset(){
        currentHealth = Mathf.FloorToInt(ResetPara.GetWithDefault("Health", 100f));
        timer = ResetPara.GetWithDefault("Timer", 0f);
    }
}
