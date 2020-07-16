using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeopleController : MonoBehaviour
{
    private int myTotalMoney = 0;
    public int money = 0;
    public GameObject myCar;
    public Transform spawnPoint;
    
    private GameObject[] peopleOutOfParkingPoints;
    private GameObject[] travellingPoints;
    private GameObject[] shopPoints;
    private GameObject entrancePoint;
    private Transform pointToGo;
    private GameObject mainexitPoint;
    private Transform ShopTarget;
    private int EnterSupermarket = 0;
    public bool flagGotCashier = false;

    public NavMeshAgent agent;
    float[] distanceFrompointtodirection;

    public Transform OuterPoint1;
    public Transform OuterPoint2;
    public Transform EndPoint;


    void Start()
    {
        money = Random.Range(200, 1000);
        myTotalMoney = money;
        flagGotCashier = false;
        EnterSupermarket = 0;
        peopleOutOfParkingPoints = GameObject.FindGameObjectsWithTag("PeopleOutOfParkingPoint");
        entrancePoint = GameObject.FindGameObjectWithTag("EntrancePoint");
        travellingPoints = GameObject.FindGameObjectsWithTag("TravellingPoint");
        shopPoints = GameObject.FindGameObjectsWithTag("ShopPoint");
        mainexitPoint = GameObject.FindGameObjectWithTag("MainExitPoint");
        checkShortestPointForOutOfParking();
        agent.SetDestination(pointToGo.position);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, pointToGo.position);
        if (distance <= 0.3f && EnterSupermarket==0)//getting closest parking entring point
        {
            float distancefromentrance = Vector3.Distance(transform.position, entrancePoint.transform.position);
            if (distance <= 0.3f)
            {
                EnterSupermarket = 1;//now going to entrance point
                //scan inner points and update on pointToGo
                ShopSellection();
                checkTravellingPoints();
                agent.SetDestination(pointToGo.position);
            }
            pointToGo = entrancePoint.transform;
            agent.SetDestination(entrancePoint.transform.position);
         }
          else if(distance <= 0.4f && EnterSupermarket==1)
             {
                 //print("Shop Changed");
                 checkTravellingPoints();
                 
             }
        else if (distance <= 0.4f && EnterSupermarket == 2)//main exit point
        {
            //select queue and enter the last queue index;
            EnterSupermarket = 3;//for queue
        }
        else if (distance <= 0.4f && EnterSupermarket == 4)//go to the car
        {
            //check if anoter point is less than car distance so first travel to that point
            if (spawnPoint == null)
                Destroy(gameObject);
            forLeaving();
            agent.SetDestination(pointToGo.position);
            EnterSupermarket = 5;
        }
        else if (distance <= 0.4f && EnterSupermarket == 5)//go to the car
        {
            //check if anoter point is less than car distance so first travel to that point
            pointToGo = spawnPoint;
            agent.SetDestination(pointToGo.position);
            EnterSupermarket = 6;
        }
        else if (distance <= 0.4f && EnterSupermarket == 6)//go to the car
        {
            if (myCar != null)
            {
                //means we reach to the car now activate carcontroller
                myCar.GetComponent<CarController>().enabled = true;
                myCar.GetComponent<NavMeshAgent>().enabled = true;
                myCar.GetComponent<CarController>().checkForAllPeople(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            //call thepeople destroy function and wait for all the people
            //then leave the car
        }
        else if (OuterPoint1 == pointToGo)
        {
            pointToGo = OuterPoint2;
            agent.SetDestination(pointToGo.position);
        }
        else if (OuterPoint2 == pointToGo)
        {
            pointToGo = EndPoint;
            agent.SetDestination(pointToGo.position);
        }
    }

    void checkShortestPointForOutOfParking()
    {
        float[] distanceFrompointtodirection = new float[peopleOutOfParkingPoints.Length];
        int d = 0;
        for (int i = 0; i < peopleOutOfParkingPoints.Length; i++)
        {
            distanceFrompointtodirection[i] = Vector3.Distance(gameObject.transform.position, peopleOutOfParkingPoints[i].transform.position);
        }
        float min = 9999;
        for (int i = 0; i < distanceFrompointtodirection.Length; i++)
        {
            if (min > distanceFrompointtodirection[i])
            {
                min = distanceFrompointtodirection[i];
                pointToGo = peopleOutOfParkingPoints[i].transform;
                
            }
        }

    }

    void forLeaving()
    {
        float[] distanceFrompointtodirection = new float[peopleOutOfParkingPoints.Length];
        int d = 0;
        for (int i = 0; i < peopleOutOfParkingPoints.Length; i++)
        {
            distanceFrompointtodirection[i] = Vector3.Distance(spawnPoint.transform.position, peopleOutOfParkingPoints[i].transform.position);
        }

        float min = 9999;
        for (int i = 0; i < distanceFrompointtodirection.Length; i++)
        {
            if (min > distanceFrompointtodirection[i])
            {
                min = distanceFrompointtodirection[i];
                pointToGo = peopleOutOfParkingPoints[i].transform;

            }
        }
    }

    public int n = 10;
    public void ShopSellection()
    {
        n--;
        if (n > 0||money>200)
        {
            shopPoints = GameObject.FindGameObjectsWithTag("ShopPoint");
            int random = Random.Range(0, shopPoints.Length);
            //check for that if have money then go
            if (money > shopPoints[random].GetComponent<ShopTrigger>().price && shopPoints[random].GetComponent<ShopTrigger>().storage > shopPoints[random].GetComponent<ShopTrigger>().piece)//ye hack ho sakta hai
                ShopTarget = shopPoints[random].transform;
            else
                ShopSellection();
            
        }
        else
        {
            //go to exit point;
            
            pointToGo=mainexitPoint.transform;
            agent.SetDestination(pointToGo.position);
            EnterSupermarket = 2;
        }

    }

    public void checkTravellingPoints()
    {
        pointToGo = ShopTarget;
        agent.SetDestination(pointToGo.position);
    }

    public void QueueCashierSet(Transform point)
    {
        pointToGo = point;
        agent.SetDestination(pointToGo.position);
    }

    public void DoCashCut(GameObject go)
    {
        //my profit myTotalMoney-money and save it
        //saveload.money += myTotalMoney - money;
        //saveload.Save();
        //GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameController>().ShowMoney();
        //wait him
        StartCoroutine(WaitForCashing(5,go));
    }

    IEnumerator WaitForCashing(float timer,GameObject go)
    {
        yield return new WaitForSeconds(timer);
        //go to min parking point
        checkShortestPointForOutOfParking();
        agent.SetDestination(pointToGo.position);
        EnterSupermarket = 4;
        go.GetComponent<CashierQueue>().space = 0;
    }

    
}
/*

        //check the distance from the shop target to my position 
        //if dis is more than distance from travelling position from my position then travel to that point then again repeat above step

        float distanceFromShop = Vector3.Distance(gameObject.transform.position, ShopTarget.position);
        //print(distanceFromShop+"min dis");

        float[] distanceFrompointtodirection = new float[travellingPoints.Length];
        int d = 0;
        
        for (int i = 0; i < travellingPoints.Length; i++)
        {
            distanceFrompointtodirection[i] = Vector3.Distance(gameObject.transform.position, travellingPoints[i].transform.position);

            if (Vector3.Distance(gameObject.transform.position, travellingPoints[i].transform.position) < 0.3f)
            {
                distanceFrompointtodirection[i] = 99999;
            }
            
        }

        //sort the array in assecind order
        for (int i = 0; i < travellingPoints.Length; i++)
        {
            for (int j = 0; j < travellingPoints.Length-1; j++)
            {
                if (distanceFrompointtodirection[j] > distanceFrompointtodirection[j + 1])
                {
                    float temp = distanceFrompointtodirection[j];
                    distanceFrompointtodirection[j] = distanceFrompointtodirection[j + 1];
                    distanceFrompointtodirection[j + 1] = temp;

                    GameObject goTemp = travellingPoints[j];
                    travellingPoints[j] = travellingPoints[j + 1];
                    travellingPoints[j + 1] = goTemp;

                }
            }
        }

        //check 2 close points and add the distance from shop
        int accuracy = 3;
        float[] distanceOfadjacentpoints = new float[accuracy];
        for (int i = 0; i < accuracy; i++)
        {
            
            distanceOfadjacentpoints[i] = distanceFrompointtodirection[i] + Vector3.Distance(ShopTarget.transform.position, travellingPoints[i].transform.position);
        }

        //check min of adjacent points
        float min = 99999;
        int index = 0;
        for (int i = 0; i < distanceOfadjacentpoints.Length; i++)
        {
            if (min > distanceOfadjacentpoints[i])
            {
                min = distanceOfadjacentpoints[i];
                index = i;
            }
        }

        pointToGo = travellingPoints[index].transform;
        if (distanceFromShop < distanceFrompointtodirection[index])
        {
            pointToGo = ShopTarget;
        }
        print("--------------------------------------------------------------");
        */
/*float min = 9999;
        int dd = 0;
        int lastindex = 0;
        int temp=0;
        for (int i = 0; i < distanceFrompointtodirection.Length; i++)
        {
            print(distanceFrompointtodirection[i]);
            if (min > distanceFrompointtodirection[i])
            {
                min = distanceFrompointtodirection[i];
                
                if (dd > 1)
                {
                    temp=index;
                    lastindex = temp;
                    d--;
                }
                dd++;
                index = i;
            }
        }
        print(lastindex+"-----------------------------------------------------" + min);
        if (distanceFromShop < min)
        {
            pointToGo = ShopTarget;
        }
        else
        {
            //check the distance of mine to that point + distance of point to that shop
            for (int i = 0; i < travellingPoints.Length; i++)
            {
                distanceFrompointtodirection[i] = distanceFrompointtodirection[i] + Vector3.Distance(ShopTarget.transform.position, travellingPoints[i].transform.position);
            }
            min = 9999;
            for (int i = 0; i < distanceFrompointtodirection.Length; i++)
            {
                if (min > distanceFrompointtodirection[i])
                {
                    min = distanceFrompointtodirection[i];
                    pointToGo = travellingPoints[i].transform;
                }
            }
           
            
        }*/