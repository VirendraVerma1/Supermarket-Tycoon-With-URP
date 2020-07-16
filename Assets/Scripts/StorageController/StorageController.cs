using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageController : MonoBehaviour
{
    public bool isStorage = false;
    public int capacity = 500;
    public string storageName;

    public GameObject[] Vans;
    public GameObject[] ContainerTriggers;
    public Transform VansSpawnPoint;

    private GameObject[] ShopSpawnPoints;

    List<GameObject> portContainers = new List<GameObject>();
    List<string> portContainerstroragename = new List<string>();

    public Transform MainIslandPortal;
    public Transform StorageIslandPortal;
    public Transform FinalPoint;
    public Transform DestroyPoint;
    private Transform shortestContainerPickPoint;

    List<GameObject> spawnVans = new List<GameObject>();

    [Header("StorageElements")]
    private GameObject[] StorageArea;
    List<Transform> emptystoragePoints=new List<Transform>();
    List<GameObject> fullstoragePoints = new List<GameObject>();

    List<StorageData> storageData = new List<StorageData>();

    public GameObject[] ProductObject;

    public GameObject StorageCanvas;
    public TextMeshProUGUI storageText;

    void Start()
    {
        ShopSpawnPoints = GameObject.FindGameObjectsWithTag("ShopPoint");
        UpdateStorageThings();
        LoadAndUpdateStorage();
        saveload.totalVansOut = saveload.mytotalvan;
    }

    void Update()
    {
        StorageCanvas.transform.LookAt(Camera.main.transform);
    }

    void UpdateStorageThings()
    {
        StorageArea = GameObject.FindGameObjectsWithTag("StorageArea");
        for (int i = 0; i < StorageArea.Length; i++)
        {
            Transform[] points = StorageArea[i].GetComponentsInChildren<Transform>();
            for (int j = 0; j < points.Length; j++)
            {
                emptystoragePoints.Add(points[j]);
            }
        }
        //print("ngc"+StorageArea.Length);
        //print("f"+emptystoragePoints.Count);
    }

    #region van and port storage

    public void AddContainerStorage(GameObject g,string name)
    {
        
        portContainers.Add(g);
        portContainerstroragename.Add(name);
        //portContainerstrorage.Add(storage);
        StartCoroutine(SpawnVans(g,name));
    }

    IEnumerator SpawnVans(GameObject scriptObject,string name)
    {
        
        while (saveload.totalVansOut > 0)
        {
            saveload.totalVansOut--;
            
            yield return new WaitForSeconds(2);
            
            CheckSHortestContainerPick(scriptObject);
            //spawn vans
            int random = Random.Range(0, Vans.Length);
            GameObject g = Instantiate(Vans[random], VansSpawnPoint.transform.position, VansSpawnPoint.transform.rotation);
            g.GetComponent<VanController>().strorageName = name;
            g.GetComponent<VanController>().containerController = scriptObject.GetComponent<ContainerOnEmpty>();
            g.GetComponent<VanController>().storageController = gameObject.GetComponent<StorageController>();
            g.GetComponent<VanController>().MainIslandPortal = MainIslandPortal;
            g.GetComponent<VanController>().SecondIslandPortal = StorageIslandPortal;
            g.GetComponent<VanController>().FinalPoint = FinalPoint;
            g.GetComponent<VanController>().SpawnPoint = VansSpawnPoint;
            g.GetComponent<VanController>().storagePickPoint = shortestContainerPickPoint;
            g.GetComponent<VanController>().DestroyPoint = DestroyPoint;
            spawnVans.Add(g);
        }
    }

    void CheckSHortestContainerPick(GameObject go)
    {
        //check from minimum distance of the trigger parking lot
        float[] distanceFromLotToPoint = new float[ContainerTriggers.Length];
        int d = 0;
        for (int i = 0; i < ContainerTriggers.Length; i++)
        {
            distanceFromLotToPoint[i] = Vector3.Distance(go.transform.position, ContainerTriggers[i].transform.position);
        }

        float min = 9999;
        for (int i = 0; i < distanceFromLotToPoint.Length; i++)
        {
            if (min > distanceFromLotToPoint[i])
            {
                min = distanceFromLotToPoint[i];
                shortestContainerPickPoint = ContainerTriggers[i].transform;
               
            }
        }
        
        
    }

    public void RemoveContainerStorage(GameObject go)
    {
        for (int i = 0; i < portContainers.Count; i++)
        {
            if (go == portContainers[i])
            {
                portContainers.RemoveAt(i);
                portContainerstroragename.RemoveAt(i);
            }
        }
    }

    public void CheckForAnotherStorage()
    {
        
        
        if (portContainers.Count > 0 && saveload.totalVansOut <= saveload.mytotalvan && saveload.totalVansOut>0)
        {
            
            int random = Random.Range(0, portContainers.Count);
            StartCoroutine(SpawnVans(portContainers[random], portContainerstroragename[random]));
        }
         
    }

    public void CheckAnotherContainerByVan(GameObject go)
    {
        if (portContainers.Count > 0)
        {
            int random = Random.Range(0, portContainers.Count);
            CheckSHortestContainerPick(portContainers[random]);
            go.GetComponent<VanController>().storagePickPoint = shortestContainerPickPoint;
        }
    }

    public void RemoveVan()
    {
        int random = Random.Range(0, spawnVans.Count);
        Destroy(spawnVans[random]);
        spawnVans.RemoveAt(random);
    }

    #endregion

    #region UpdateStorage

    public void LoadAndUpdateStorage()
    {
        storageData.Clear();

        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            storageData.Add(new StorageData(saveload.shopName[i], saveload.shopStorage[i], saveload.shopMaxStorage[i]));
        }
        CalculateAndShowStorage();
    }

    public void AddStorageFromVan(string storageName,int capacity)
    {
        //print("3");
        bool flag=false;
        for (int i = 0; i < storageData.Count; i++)
        {
            if (storageData[i].StorageName == storageName)
            {
                if (storageData[i].StorageQuantity < storageData[i].StorageMaxCapacity)
                {
                    if ((storageData[i].StorageQuantity + capacity) > storageData[i].StorageMaxCapacity)
                    {
                        storageData[i].StorageQuantity = storageData[i].StorageMaxCapacity;
                    }
                    else
                    {
                        storageData[i].StorageQuantity += capacity;
                    }
                }
                flag = true;
            }
        }

        if (!flag)
        {
            
            storageData.Add(new StorageData(storageName, capacity, 2000));
        }
        CalculateAndShowStorage();

    }

    void CalculateAndShowStorage()
    {
        //print("2");
        int totalStorage=0;
        int maxStorage=saveload.totalStorage;
        for (int i = 0; i < storageData.Count; i++)
        {
            totalStorage += storageData[i].StorageQuantity;
            //maxStorage+=storageData[i].StorageMaxCapacity;
        }
        saveload.occupiedStorage = totalStorage;
        storageText.text = totalStorage.ToString() + "/" + maxStorage.ToString();
        AddCubesInStorage(totalStorage, maxStorage); 
    }

    void AddCubesInStorage(int storedProduct,int MaxStorage)
    {
       
        float limit = (float)MaxStorage / (float)emptystoragePoints.Count;
        //print("MaxStorage " + MaxStorage);
        //print("emptystoragePoints " + emptystoragePoints.Count);
        //print("storedProduct " + storedProduct);
        //print("Limit "+limit);
        //ClearAllData
        for(int i=0;i<fullstoragePoints.Count;i++)
        {
            Destroy(fullstoragePoints[i]);
        }
        //print("1+" +emptystoragePoints.Count);
        //store data
         //int s=storedProduct/limit;
        //print("Stored "+s);
        //print(storageData.Count);
        for(int i=0;i<storageData.Count;i++)
        {
            float num=(float)storageData[i].StorageQuantity/(float)limit;
            //print("4|"+num);
            //print("Num" + (int)num);
            for(int j=0;j<(int)num;j++)
            {
                GameObject go=Instantiate(ProductObject[i], emptystoragePoints[j].transform.position, emptystoragePoints[j].transform.rotation);
                fullstoragePoints.Add(go);
                //print("3");
            }

            //store in the savefile
            if (storageData[i].StorageName == saveload.shopName[i])
            {
                saveload.shopStorage[i] = storageData[i].StorageQuantity;
            }
        }

        GameObject[] ggo = GameObject.FindGameObjectsWithTag("ShopPoint");
        foreach (GameObject g in ggo)
        {
            g.GetComponent<ShopTrigger>().UpdateStorage();
        }

    }

    #endregion
}
