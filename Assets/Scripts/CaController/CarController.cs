using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    private GameObject StartingFixedPoint;
    private GameObject FinalFixedPoint;
    private Transform FinalEndPoint;
    private GameObject[] StartingFinalPoints;
    private GameObject[] AllParkingTriggers;
    private GameObject[] AllParkingDirectionPoints;
    private GameObject[] AllParkingPoints;
    private GameObject[] FinalTurnPoints;


    public int totalPeople = 1;

    public GameObject BookedparkingLotForCar;

    public NavMeshAgent agent;

    private Transform anotherPointToFindDistance;
    private GameObject closestparkingPoint;
    private GameObject closestparkingDirectionPoint;
    private Transform closefinalPoint;

    private int leaving = 0;
    private int myparkingPosition = 0;
    //0 = not leaving
    //1 = back to final point
    //2 = to end point

    // Start is called before the first frame update
    void Start()
    {
        //take all the required information from the environment
        StartingFixedPoint = GameObject.FindGameObjectWithTag("StartingFixedPoint");
        FinalFixedPoint = GameObject.FindGameObjectWithTag("FinalFixedPoint");
        FinalEndPoint = GameObject.FindGameObjectWithTag("FinalEndPoint").transform;
        //StartingFinalPoints = GameObject.FindGameObjectsWithTag("StartingFixedPoints");
        AllParkingTriggers = GameObject.FindGameObjectsWithTag("ParkingLot");
        AllParkingDirectionPoints = GameObject.FindGameObjectsWithTag("TurningPoinsCar");
        AllParkingPoints = GameObject.FindGameObjectsWithTag("ParkingPoints");
        FinalTurnPoints = GameObject.FindGameObjectsWithTag("FinalTurnPointsCar");

        agent.enabled = true;
        //agent=gameObject.GetComponent<NavMeshAgent>();
        //agent.Warp(StartingFinalPoints[0].transform.position);
        Vector3 destination = StartingFixedPoint.transform.position;
        agent.SetDestination(destination);
        
        anotherPointToFindDistance = StartingFixedPoint.transform;
        CheckNearestParkingLotPoint();
        CheckNearestTurningPoint();
        SearchForFinalTurnPoint();
        CheckMyParkingPosition();

        gameObject.GetComponent<BoxCollider>().enabled = false;

        //print(AllParkingTriggers.Length);
        //print(myparkingPosition);
        //print("Left"+(myparkingPosition / 8) % 2);
        
    }

    void Update()
    {
        float distance=Vector3.Distance(transform.position,anotherPointToFindDistance.position);
        if (distance <= 0.6f)
        {
            if (leaving == 2)
            {
                //now call
                leaving = 3;
                BookedparkingLotForCar.GetComponent<ParkingLot>().lotBooked = 0;
                anotherPointToFindDistance = closefinalPoint;
                Vector3 destination = closefinalPoint.transform.position;
                agent.SetDestination(closefinalPoint.transform.position);
                
            }
            else if(leaving==3)
            {
                agent.SetDestination(FinalEndPoint.position);
            }
            else if (anotherPointToFindDistance == StartingFixedPoint.transform)
            {
                anotherPointToFindDistance = FinalFixedPoint.transform;
                agent.SetDestination(FinalFixedPoint.transform.position);
            }
            else if (anotherPointToFindDistance == FinalFixedPoint.transform)
            {
                //now check for nearest turning point
                
                anotherPointToFindDistance = closestparkingDirectionPoint.transform;
                agent.SetDestination(closestparkingDirectionPoint.transform.position);

            }
            else if (anotherPointToFindDistance == closestparkingDirectionPoint.transform)
            {
                anotherPointToFindDistance = closestparkingPoint.transform;
                agent.SetDestination(closestparkingPoint.transform.position);
            }
            else if (anotherPointToFindDistance == closestparkingPoint.transform)
            {
                anotherPointToFindDistance = BookedparkingLotForCar.transform;
                gameObject.GetComponent<BoxCollider>().enabled = true;
                agent.SetDestination(BookedparkingLotForCar.transform.position);
            }
        }

        if (leaving == 1)
        {
            leaving = 2;
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            anotherPointToFindDistance = closestparkingPoint.transform;
            agent.SetDestination(closestparkingPoint.transform.position);
            
        }
        

        /*
      
        if (leaving == 1)
        {
            gameObject.transform.Translate(0, 0, -0.01f);//gameObject.transform.Translate(0, 0.5f, -0.01f);
            if (((myparkingPosition+1)/8)%2==0)
                gameObject.transform.Rotate(0, 0.5f, 0);
            else
                gameObject.transform.Rotate(0, -0.5f, 0);
            if (distance >= 1.5f)
            {
                
                leaving = 3;
                BookedparkingLotForCar.GetComponent<ParkingLot>().lotBooked = 0;
                anotherPointToFindDistance = closefinalPoint;
                Vector3 destination = closefinalPoint.transform.position;
                agent.SetDestination(closefinalPoint.transform.position);
                
            }
        }
         * 
         * */
    }

    void CheckMyParkingPosition()
    {
        for (int i = 0; i < AllParkingTriggers.Length; i++)
        {
            if (BookedparkingLotForCar == AllParkingTriggers[i])
                myparkingPosition = i;
        }
    }

    void CheckNearestParkingLotPoint()
    {
        //check from minimum distance of the trigger parking lot
        float[] distanceFromLotToPoint = new float[AllParkingPoints.Length];
        int d=0;
        for (int i = 0; i < AllParkingPoints.Length; i++)
        {
            distanceFromLotToPoint[i] = Vector3.Distance(BookedparkingLotForCar.transform.position, AllParkingPoints[i].transform.position);
        }
            
        float min=9999;
        for(int i=0;i<distanceFromLotToPoint.Length;i++)
        {
            if (min > distanceFromLotToPoint[i])
            {
                min = distanceFromLotToPoint[i];
                closestparkingPoint = AllParkingPoints[i];
            }
        }
        
    }

    void CheckNearestTurningPoint()
    {
        //find the distance between closestparkingPoint and alldirection turning points
        float[] distanceFrompointtodirection = new float[AllParkingDirectionPoints.Length];
        int d = 0;
        for (int i = 0; i < AllParkingDirectionPoints.Length; i++)
        {
            distanceFrompointtodirection[i] = Vector3.Distance(closestparkingPoint.transform.position, AllParkingDirectionPoints[i].transform.position);
        }

        float min = 9999;
        for (int i = 0; i < distanceFrompointtodirection.Length; i++)
        {
            if (min > distanceFrompointtodirection[i])
            {
                min = distanceFrompointtodirection[i];
                closestparkingDirectionPoint = AllParkingDirectionPoints[i];
            }
        }
    }

    

    public void checkForAllPeople(GameObject go)
    {
        totalPeople--;
        Destroy(go);
        if (totalPeople <= 0)
        {
            //exit the car
            leaving = 1;
            anotherPointToFindDistance = closestparkingPoint.transform;
        }
    }

    void SearchForFinalTurnPoint()
    {
        float[] distanceFromLotToPoint = new float[FinalTurnPoints.Length];
        int d = 0;
        for (int i = 0; i < FinalTurnPoints.Length; i++)
        {
            distanceFromLotToPoint[i] = Vector3.Distance(closestparkingPoint.transform.position, FinalTurnPoints[i].transform.position);
        }

        float min = 9999;
        for (int i = 0; i < distanceFromLotToPoint.Length; i++)
        {
            if (min > distanceFromLotToPoint[i])
            {
                min = distanceFromLotToPoint[i];
                closefinalPoint = FinalTurnPoints[i].transform;
            }
        }
    }

}
