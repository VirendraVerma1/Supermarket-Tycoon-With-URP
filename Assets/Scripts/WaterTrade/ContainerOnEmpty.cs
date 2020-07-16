using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerOnEmpty : MonoBehaviour
{
    public GameObject ShoipTrigger;
    public GameObject PortManager;
    public GameObject StorageController;

    public int storageQuantity;
    private GameObject containerLoaded;

    public GameObject Controller;
    public GameController gcontroller;

    private int ContainerStorageCapacity;
    private string ContainerCountryname;
    private string ContainerStorageName;


    void Start()
    {
       // Controller = GameObject.FindGameObjectWithTag("GameManager").gameObject;
    }

    void OnTriggerEnter(Collider col)
    {
        col.gameObject.transform.parent = null;
        PortManager.GetComponent<PortManager>().HandleUp();
        PortManager.GetComponent<PortManager>().haveContainer = false;
        //StorageController.GetComponent<StorageController>().isStorage = true;
        containerLoaded = col.gameObject;
        //saveload.money -= col.gameObject.GetComponent<ContainerController>().tradePrice;//TODO show some effects
        //saveload.Save();

        storageQuantity = col.gameObject.GetComponent<ContainerController>().storageQ;
        /*
        GameObject[] go = GameObject.FindGameObjectsWithTag("ShopPoint");
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].GetComponent<ShopTrigger>().storageName == col.gameObject.GetComponent<ContainerController>().storageName)
            {
                storageQuantity = col.gameObject.GetComponent<ContainerController>().storageQ;//shift this code when van service is intoduced
            }
        }
        */
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<StorageController>().AddContainerStorage(gameObject, col.gameObject.GetComponent<ContainerController>().storageName);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameController>().ShowMoney();
        //Destroy(col.gameObject, 10f);//destroy after taking out all items
    }

    public int UpdateQuantity(int capacity)
    {
        if (containerLoaded != null)
        {
            print("Have Container");
            int tempStorage = 0;
            if (storageQuantity < capacity)
            {
                tempStorage = storageQuantity;
                storageQuantity =0;
                containerLoaded.GetComponent<ContainerController>().SetText(storageQuantity);
                if (storageQuantity <= 0)
                {
                    Controller.GetComponent<StorageController>().RemoveContainerStorage(containerLoaded);
                    StartCoroutine(WaitToDeleteContainer());
                }
                return tempStorage;
            }
            else
            {
                storageQuantity -= capacity;
                containerLoaded.GetComponent<ContainerController>().SetText(storageQuantity);
                if (storageQuantity <= 0)
                {
                    Controller.GetComponent<StorageController>().RemoveContainerStorage(containerLoaded);
                    StartCoroutine(WaitToDeleteContainer());
                }
                return capacity;
            }
        }
        else
        {
            print("Object deleted return to home");
            return 0;
        }
        
    }

    IEnumerator WaitToDeleteContainer()
    {
        yield return new WaitForSeconds(1f);
        ShoipTrigger.GetComponent<PortTrigger>().occupied = 0;
        
        
        string countryName = containerLoaded.GetComponent<ContainerController>().storageCountry;
        string storageName = containerLoaded.GetComponent<ContainerController>().storageName;
        int sQ = containerLoaded.GetComponent<ContainerController>().storageQ;
        Controller.GetComponent<GameController>().RemoveTrade(countryName, storageName, sQ);

        Destroy(containerLoaded);
    }
}
