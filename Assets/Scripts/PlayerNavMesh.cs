using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine. AI;
public class PlayerNavMesh : MonoBehaviour {
    [SerializeField] private Transform movePositionTransform;
    private NavMeshAgent agent;
    public Transform house, inside;
    
    public bool followObject;
    
    public float walkPointRange;
    public Vector3 walkPoint, distanceToWalkPoint;
    public bool walkPointSet;

    public float sightRange;

    public int numStoredWalkpoints;
    public int previousWalkPointsIndex;
    public int previousWalkPointsRange;
    private Vector3[] previousWalkPoints;
    public bool alreadySearched;
    
    public bool alreadySearched0, alreadySearched1, alreadySearched2;
    public float distanceMag0, distanceMag1, distanceMag2;

    public Vector3 distance;
    public float distanceMag;

    private bool houseInSight, insideInSight;
    
    public LayerMask whatIsGround, whatIsHouse, whatIsInside, whatIsDoor;
    
    private bool atHouse, insideHouse;

    private bool exploringH, gTHouse, exploring;
    
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        previousWalkPoints = new Vector3[numStoredWalkpoints];
        previousWalkPointsIndex = 0;
        alreadySearched = false;
        alreadySearched0 = false;
        alreadySearched1 = false;
        alreadySearched2 = false;
    }
    
    private void Update() {
        houseInSight = Physics.CheckSphere(transform.position, sightRange, whatIsHouse);
        insideInSight = Physics.CheckSphere(transform.position, sightRange, whatIsDoor);
        atHouse = Physics.Raycast(transform.position, -transform.up, 2f, whatIsHouse);

        

        if(followObject){
            agent.destination = movePositionTransform.position;
        } else {
            if(Physics.Raycast(transform.position, -transform.up, 2f, whatIsInside)){
                Exploring(whatIsInside);
                insideHouse = true;
            } else if(insideInSight){
                goInside();
            }else if(Physics.Raycast(transform.position, -transform.up, 2f, whatIsHouse)){
                Exploring(whatIsHouse);
                
            } else if(houseInSight && !atHouse){
                goToHouse();
            } else{
                Exploring(whatIsGround);
            }
            
        }
    }

    private void Exploring(LayerMask region){
        if (!walkPointSet) SearchWalkPoint(region);
        if(walkPointSet){
            agent.SetDestination(walkPoint);
        } 
        
        distanceToWalkPoint = transform.position - walkPoint;



        if(distanceToWalkPoint.magnitude > 1.5*walkPointRange || !(Physics.Raycast(walkPoint, -transform.up, 2f, region))){
            walkPointSet = false;
        }

        if(distanceToWalkPoint.magnitude < 1f ){
            walkPointSet = false;

        }
    }

    private void SearchWalkPoint(LayerMask region){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        alreadySearched = false;
        for(int i = 0; i < numStoredWalkpoints; i++){
            distance = walkPoint - previousWalkPoints[i];
            distanceMag = distance.magnitude;
            alreadySearched = alreadySearched | (distance.magnitude < (2*previousWalkPointsRange));
        }

        // Vector3 distance0 = walkPoint - previousWalkPoints[0];
        // distanceMag0 = distance0.magnitude;
        // alreadySearched0 = distanceMag0 < 2*previousWalkPointsRange;

        // Vector3 distance1 = walkPoint - previousWalkPoints[1];
        // distanceMag1 = distance1.magnitude;
        // alreadySearched1 = distanceMag1 < 2*previousWalkPointsRange;

        // Vector3 distance2 = walkPoint - previousWalkPoints[2];
        // distanceMag2 = distance2.magnitude;
        // alreadySearched2 = distanceMag2 < 2*previousWalkPointsRange;
        
        distanceToWalkPoint = transform.position - walkPoint;

        if(Physics.Raycast(walkPoint, -transform.up, 2f, region) && !alreadySearched && (distanceToWalkPoint.magnitude < walkPointRange)){

            previousWalkPoints[previousWalkPointsIndex] = walkPoint;

            previousWalkPointsIndex++;
            if(previousWalkPointsIndex == numStoredWalkpoints){
                previousWalkPointsIndex = 0;
            }
            walkPointSet = true;
        }
    }

    private void goToHouse(){
        agent.SetDestination(house.position);
        Vector3 distanceToWalkPoint = transform.position - house.position;
        
        if(distanceToWalkPoint.magnitude < 1f){
            atHouse = true;
        }
    }

    private void goInside(){
        agent.SetDestination(inside.position);
    }
    private void OnDrawGizmos(){

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(walkPoint, previousWalkPointsRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(previousWalkPoints[1], previousWalkPointsRange);
        Gizmos.DrawWireSphere(previousWalkPoints[2], previousWalkPointsRange);
        Gizmos.DrawWireSphere(previousWalkPoints[0], previousWalkPointsRange);
    }
}


