using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaxiController : MonoBehaviour
{
    private GameObject StartingFixedPoint;
    private GameObject FinalFixedPoint;
    private Transform FinalEndPoint;

    private GameObject GameManager;

    public NavMeshAgent agent;
    private Transform anotherPointToFindDistance;
    private int indexWaypoint=0;
    Vector3 destination;
    void Start()
    {
        StartingFixedPoint = GameObject.FindGameObjectWithTag("StartingFixedPoint");
        FinalFixedPoint = GameObject.FindGameObjectWithTag("FinalFixedPoint");
        FinalEndPoint = GameObject.FindGameObjectWithTag("FinalEndPoint").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameManager");


        agent.enabled = true;
        //destination = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint].position;
        //agent.SetDestination(destination);
        destination = StartingFixedPoint.transform.position;
        agent.SetDestination(destination);
        anotherPointToFindDistance = StartingFixedPoint.transform;
        //print(GameManager.GetComponent<GameController>().TaxiWaypoints.Length);
    }

    
    void Update()
    {
        float distance=Vector3.Distance(transform.position,anotherPointToFindDistance.position);
        if (distance <= 0.6f)
        {
            if (anotherPointToFindDistance == StartingFixedPoint.transform)
            {
                indexWaypoint = 0;
                anotherPointToFindDistance = FinalFixedPoint.transform;
                agent.SetDestination(FinalFixedPoint.transform.position);
            }
            else if (anotherPointToFindDistance == FinalFixedPoint.transform)
            {
                anotherPointToFindDistance = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint];
                destination = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint].position;
                agent.SetDestination(destination);
                
            }
            else if ((anotherPointToFindDistance == GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint]) && ((GameManager.GetComponent<GameController>().TaxiWaypoints.Length-1)>indexWaypoint))
            {
                indexWaypoint++;
                anotherPointToFindDistance = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint];
                destination = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint].position;
                agent.SetDestination(destination);
                //print(indexWaypoint);
            }
            else
            {
                anotherPointToFindDistance = FinalEndPoint.transform;
                agent.SetDestination(FinalEndPoint.transform.position);
            }
        }
    }

    public void ReactivateNavAgent()
    {
        anotherPointToFindDistance = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint];
        destination = GameManager.GetComponent<GameController>().TaxiWaypoints[indexWaypoint].position;
        agent.SetDestination(destination);
    }
}
