using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SimpleAi : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer, whatIsExplore;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;


    public float sightRange, exploreRange;
    public bool playerInSightRange, playerInExploreRange;

    bool atJar;
    private void Awake(){
        player = GameObject.Find("Jar").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update(){
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInExploreRange = Physics.CheckSphere(transform.position, exploreRange, whatIsExplore);
        
        Patrolling();
        //ExploreJar();
        // if(!playerInSightRange && !playerInExploreRange) Patrolling();
        // if(playerInSightRange && !playerInExploreRange) FindJar();
        // if(playerInSightRange && playerInExploreRange) ExploreJar();
    }

    private void Patrolling(){
        if (!walkPointSet) SearchWalkPoint_P();
        if(walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f){
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint_P(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);
        walkPointSet = true;
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void FindJar(){
        agent.SetDestination(player.position);
    }

    private void ExploreJar(){
        if (!walkPointSet) SearchWalkPoint_E();
        if(walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < .1f){
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint_E(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsExplore)){
            walkPointSet = true;
        }
    }
    private void Stop(){
        agent.SetDestination(transform.position);


    }

}
