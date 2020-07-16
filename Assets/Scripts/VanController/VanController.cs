using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class VanController : MonoBehaviour
{
    public GameObject Canvas;
    public TextMeshProUGUI storageText;

    public string strorageName;
    public NavMeshAgent agent;
    public int maxcapacity=500;
    public int pickedCapacity = 0;
    public StorageController storageController;

    public ContainerOnEmpty containerController;

    public Transform storagePickPoint;
    public Transform MainIslandPortal;
    public Transform SecondIslandPortal;
    public Transform FinalPoint;
    public Transform SpawnPoint;

    public Transform DestroyPoint;

    private Transform myCurrentTarget;
    float distance;
    bool goHome = false;

    int phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        maxcapacity = saveload.vanCapacity;
        storageText.text = pickedCapacity.ToString() + "/" + maxcapacity.ToString();


        agent.enabled = true;
        myCurrentTarget = storagePickPoint;
        agent.SetDestination(storagePickPoint.position);
    }

   

    // Update is called once per frame
    void Update()
    {
        Canvas.transform.LookAt(Camera.main.transform);

        distance = Vector3.Distance(gameObject.transform.position, myCurrentTarget.position);
        

        if (!goHome)
        {
            if ((storagePickPoint == null || containerController.storageQuantity <= 0) && pickedCapacity==0)
            {
                print("No Home" + storagePickPoint.name + "|" + containerController.storageQuantity + "|" + pickedCapacity);
                storagePickPoint = DestroyPoint;
                goHome = true;

                if (phase == 4 ||phase==1||phase==2||phase==0)
                {
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    myCurrentTarget = DestroyPoint;
                    storagePickPoint = DestroyPoint;
                    agent.SetDestination(DestroyPoint.position);
                }
                else
                {
                    myCurrentTarget = SecondIslandPortal;
                    agent.SetDestination(SecondIslandPortal.position);
                    print("Storage Quantity"+containerController.storageQuantity);
                    
                }

                
                //myCurrentTarget = SpawnPoint;
                //saveload.totalVansOut++;
                //Destroy(gameObject);
                //agent.SetDestination(SpawnPoint.position);
            }
        }

        if (distance < 1f)
        {
            if (myCurrentTarget == DestroyPoint && DestroyPoint == storagePickPoint)
            {
                saveload.totalVansOut++;
                storageController.CheckForAnotherStorage();
                Destroy(gameObject);
            }
            if (myCurrentTarget == storagePickPoint)                        //picking the product
            {
                //cut capacity from container
                //print("////////////////////////");
                //print("Before" + pickedCapacity);
                pickedCapacity = containerController.UpdateQuantity(maxcapacity);
                storageText.text = pickedCapacity.ToString() + "/" + maxcapacity.ToString();
                //print("After" + pickedCapacity);
                //for ending the picking race
                if (pickedCapacity < maxcapacity)
                {
                    storagePickPoint = DestroyPoint;
                    //storageController.CheckAnotherContainerByVan(gameObject);
                }
                phase = 1;
                myCurrentTarget = MainIslandPortal;
                agent.SetDestination(MainIslandPortal.position);
                //StartCoroutine(WaitToLoadUnload());
            }
            else if (myCurrentTarget == MainIslandPortal)
            {
                myCurrentTarget = FinalPoint;
                //transport to secondIslandPortal
                agent.enabled = false;
                gameObject.transform.position = SecondIslandPortal.position;
                agent.enabled = true;
                agent.SetDestination(FinalPoint.position);
                //print("1");
                phase = 2;
            }
            else if (myCurrentTarget == FinalPoint)                            //storing the product
            {
                myCurrentTarget = SecondIslandPortal;

                //call to store the product
                storageController.AddStorageFromVan(strorageName,pickedCapacity);

                //unload and add to storage
                pickedCapacity = 0;
                storageText.text = pickedCapacity.ToString() + "/" + maxcapacity.ToString();

                //print("2");

                agent.SetDestination(SecondIslandPortal.position);
                //StartCoroutine(WaitToLoadUnload());
                phase = 3;
            }
            else if (myCurrentTarget == SecondIslandPortal)
            {
                myCurrentTarget = storagePickPoint;
                //transport to main land
                agent.enabled = false;
                gameObject.transform.position = MainIslandPortal.position;
                agent.enabled = true;
                
                agent.SetDestination(storagePickPoint.position);
                //print("4");
                phase = 4;

                if(storagePickPoint==DestroyPoint)
                {
                    myCurrentTarget = DestroyPoint;
                    //storageController.CheckAnotherContainerByVan(gameObject);
                    agent.SetDestination(DestroyPoint.position);
                    
                }

            }
            
            
        }
    }

    IEnumerator WaitToLoadUnload()
    {
        yield return new WaitForSeconds(2f);
        agent.SetDestination(myCurrentTarget.position);
    }

}
