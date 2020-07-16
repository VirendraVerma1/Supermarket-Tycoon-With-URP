using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public GameObject FinalPortStop;
    public GameObject ExitStop;
    public NavMeshAgent agent;

    private GameObject[] ShipTurnPoints;

    private GameObject ClosestTurningPoint;
    private Transform anotherPointToFindDistance;
    public int stage=0;

    public Text[] Timers;
    int index = 0;
    int sec;
    int min;
    float time;
    float finalDistance;
    float distance;
    public GameController gc;

    void Start()
    {
        ShipTurnPoints = GameObject.FindGameObjectsWithTag("ShipTurnPoint");
        ExitStop = GameObject.FindGameObjectWithTag("LastShipExit");
        CheckClosestTurnPoint();
        agent.SetDestination(ClosestTurningPoint.transform.position);
        stage=1;
        gc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameController>();
    }
    float counter = 1;
    void Update()
    {
        distance = Vector3.Distance(transform.position, anotherPointToFindDistance.position);
        
        counter -= Time.deltaTime;
        if (counter < 0 & stage==1)
        {
            counter = 1;
            finalDistance = Vector3.Distance(transform.position, FinalPortStop.transform.position);
            time = (finalDistance / agent.speed)-4;
            
            min = (int)time / 60;
            sec = (int)time - (int)min * 60;
            gc.UpdateTimerShips(index, min, sec);
        }
        if (distance <= 2f)
        {
            if (stage == 1)
            {
                
                
                anotherPointToFindDistance = FinalPortStop.transform;
                Vector3 distances = FinalPortStop.transform.position;
                agent.SetDestination(FinalPortStop.transform.position);
                
            }
            
        }
        if (stage == 2)
        {
            
            stage = 3;
            agent.SetDestination(ExitStop.transform.position);
            anotherPointToFindDistance = ExitStop.transform;
        }
    }

    void CheckClosestTurnPoint()
    {
        float[] distanceFromLotToPoint = new float[ShipTurnPoints.Length];
        int d = 0;
        for (int i = 0; i < ShipTurnPoints.Length; i++)
        {
            distanceFromLotToPoint[i] = Vector3.Distance(FinalPortStop.transform.position, ShipTurnPoints[i].transform.position);
        }

        float min = 9999;
        for (int i = 0; i < distanceFromLotToPoint.Length; i++)
        {
            if (min > distanceFromLotToPoint[i])
            {
                min = distanceFromLotToPoint[i];
                ClosestTurningPoint = ShipTurnPoints[i];
                anotherPointToFindDistance = ClosestTurningPoint.transform;
                index = i;
            }
        }
    }
    
}
