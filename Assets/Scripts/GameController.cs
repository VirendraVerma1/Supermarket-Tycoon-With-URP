     using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
    [Header("Taxi Waypoints")]
    public Transform[] TaxiWaypoints;
    public Transform TaxiPoint;
    public GameObject TaxiGO;

    [Header("Car Elements")]
    public GameObject[] CarGameObjects;
    public GameObject[] PeoplesGameObject;
    public GameObject[] Shop;
    public Transform CarSpawnPoint;

    private GameObject[] AllParkingTriggers;

    private float carSpawnTimeInterval;
    private float carSpawnTimer;
    private GameObject BookedparkingLotGO;
    bool lotfound;
    GameController gc;
    List<ShopData> shopData = new List<ShopData>();
    
    int n = 1;
    int myLand = 1;

    public bool isPannelActive = false;

    void Awake()
    {
        saveload.Load();
        myLand = saveload.landNo;
        saveload.totalVansOut = saveload.mytotalvan;
    }

    void Start()
    {
         
        //Load Data From The Save File
        
        
        CheckMyLand();

        //Load All the Shops


        gc = gameObject.GetComponent< GameController>();
        //updateMoney from load
        
        ShowMoney();
        ShopUI.SetActive(false);

        lotfound = true;
        carSpawnTimeInterval = 3;
        carSpawnTimer = 1;

        //Load objects from the environment
        AllParkingTriggers = GameObject.FindGameObjectsWithTag("ParkingLot");
        //ClearTradeData();

        //StartCoroutine(CountEarning());
        SetThingsUp();
        StaticsPannel.SetActive(false);
        CheckForTaxiPort();
        StartCoroutine(SpawnRandomPeopleStarting());
    }
    float count = 500;
    

    void Update()
    {
        //car spawning
        carSpawnTimer -= Time.deltaTime;

        if (carSpawnTimer < 0)
        {
            carSpawnTimer = carSpawnTimeInterval;
            CheckForEmptyParkingLot();
            if (lotfound)
            {
                int random = UnityEngine.Random.RandomRange(0, CarGameObjects.Length);
                GameObject go = Instantiate(CarGameObjects[random], CarSpawnPoint.transform.position, CarSpawnPoint.transform.rotation);
                go.GetComponent<CarController>().BookedparkingLotForCar = BookedparkingLotGO;
                BookedparkingLotGO.GetComponent<ParkingLot>().lotBooked = 1;
                
            }
            
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            count = 500;
        }
        count -= 1;
        //shop hit
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (count > 494 && !isPannelActive)
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.tag == "Shop")
                    {
                        GameObject go = hit.collider.gameObject;
                        GameObject g = go.transform.Find("Shop").gameObject;
                        ShopUI.SetActive(true);
                        isPannelActive = true;
                        UpdateShopUI(g);
                    }
                    else if (hit.collider.tag == "Parking")
                    {
                        isPannelActive = true;
                        gameObject.GetComponent<ParkingUpgrade>().OpenParkingPannel();
                    }
                    else if (hit.collider.tag == "Office")
                    {
                        isPannelActive = true;
                        gameObject.GetComponent<OfficeUpgrade>().OpenOfficePannel();
                    }
                    else if (hit.collider.tag == "PortOffice")
                    {
                        isPannelActive = true;
                        gameObject.GetComponent<PortOffice>().OpenPortOfficePannel();
                    }
                    else if (hit.collider.tag == "StorageArea" || hit.collider.tag == "Storage")
                    {
                        isPannelActive = true;
                        gameObject.GetComponent<StorageUpdate>().OnStoragePannelOpen();

                    }
                }

            }

            count = 500;

        }
    }

    public void DisableActivePannel()
    {
        StartCoroutine(WaitAndDisablePannel());
    }

    IEnumerator WaitAndDisablePannel()
    {
        yield return new WaitForSeconds(0.2f);
        isPannelActive = false;
    }

    #region earningCalculation

    [Header("Statics")]
    public GameObject StaticsPannel;
    public Text StaticsEarningText;
    public Text StaticsServiceText;
    public Text StaticsTotalText;

    public Text ParkingEarningText;
    public Text PortServiceText;

    public GameObject[] StaticsShopGameObjects;
    public Text[] StaticsShopNameText;
    public Text[] StaticsShopEarningText;

    public void OnStaticButtonPressed()
    {
        StaticsPannel.SetActive(true);
        isPannelActive = true;
    }

    public void OnStaticsCloseButtonPressed()
    {
        StaticsPannel.SetActive(false);
        isPannelActive = false;
    }

    void SetThingsUp()
    {
        //StaticsEarningText
        //UpdateStaticsText();
        StartCoroutine(CountEarning());
    }

    int totalEarning;
    int totalSpenditure;
    int weekCounter=0;
    GameObject[] g;
    IEnumerator CountEarning()
    {
        totalEarning = 0;
        totalSpenditure = 0;
        while (n > 0)
        {
            weekCounter++;
            
            UpdateStaticsText();

            if (weekCounter >= 120)
            {
                //cut the money
                
                saveload.money+=(totalEarning - totalSpenditure);
                weekCounter = 0;
                TradeRefreshData();
            }
            yield return new WaitForSeconds(2);
        }
    }

    void UpdateStaticsText()
    {

        totalEarning = 0;
        totalSpenditure = 0;
        TradeQueueCounter();
        //Earning Part
        totalEarning += saveload.parkingEarning;    //adding parking money
        g = GameObject.FindGameObjectsWithTag("ShopPoint");
        for (int i = 0; i < g.Length; i++)
        {
            totalEarning += g[i].GetComponent<ShopTrigger>().shopEarning;

            if (weekCounter >= 120)
            {
                g[i].GetComponent<ShopTrigger>().shopEarning = 0;
            }
            employeeSalary = saveload.shopEmployeSalary[i];
            totalSpenditure += g[i].GetComponent<ShopTrigger>().employee * employeeSalary * g[i].GetComponent<ShopTrigger>().employee;

            StaticsShopGameObjects[i].SetActive(true);
            StaticsShopNameText[i].text = saveload.shopName[i];
            StaticsShopEarningText[i].text = (g[i].GetComponent<ShopTrigger>().shopEarning - (g[i].GetComponent<ShopTrigger>().employee * employeeSalary * g[i].GetComponent<ShopTrigger>().employee)).ToString();

            if ((g[i].GetComponent<ShopTrigger>().shopEarning - (g[i].GetComponent<ShopTrigger>().employee * employeeSalary * g[i].GetComponent<ShopTrigger>().employee)) < 0)
                StaticsShopEarningText[i].color = Color.red;
            else
                StaticsShopEarningText[i].color = Color.green;
        }

        //Expenditure Part
        totalSpenditure += saveload.serviceCharge;

        //set all text
        StaticsEarningText.text = totalEarning.ToString();
        StaticsServiceText.text = totalSpenditure.ToString();
        StaticsTotalText.text = (totalEarning - totalSpenditure).ToString();

        if ((totalEarning - totalSpenditure) < 0)
            StaticsTotalText.color = Color.red;
        else
            StaticsTotalText.color = Color.green;

        ParkingEarningText.text = saveload.parkingEarning.ToString();
        PortServiceText.text = "-"+saveload.serviceCharge.ToString();
    }

    #endregion

    #region ShopUI

    [Header("ShopUI")]
    public GameObject ShopUI;
    private GameObject SelectedShop;

    public Text ShopNametext;
    public Text ShopEmployeetext;
    public Text ShopDiscounttext;
    public Text ShopPricetext;
    public Text ShopPopularitytext;
    public Text ShopPerPiecetext;
    public Text TotalPieceSold;

    public Text LevelShoptext;
    public Image LevelBartext;
    public Text Upgradetext;
    public Text EmployeeShoptext;
    public Text DiscountShopext1;
    public Text DiscountShopext2;

    public Sprite EmployeeSprite;
    public Sprite NoEmployeeSprite;

    public Image[] EmployeeImage;

    private int EmployeeNumber = 1;

    public Text EarningText;
    public Text ServiceText;
    public Text TotalText;
    int earningshopLoop;

    [Header("Shop Upgrades")]
    public GameObject[] Shop1Upgrades;
    public GameObject[] Shop2Upgrades;
    public GameObject[] Shop3Upgrades;
    public GameObject[] Shop4Upgrades;

    void UpdateShopUI(GameObject g)
    {
        isPannelActive = true;
        SelectedShop=g;
        ShopNametext.text = SelectedShop.GetComponent<ShopTrigger>().storageName + " (Level " + SelectedShop.GetComponent<ShopTrigger>().level.ToString()+")";
        ShopEmployeetext.text = "Employee " + SelectedShop.GetComponent<ShopTrigger>().employee.ToString();
        ShopDiscounttext.text = "Discount " + SelectedShop.GetComponent<ShopTrigger>().discount.ToString() + "%"; ;
        ShopPricetext.text = SelectedShop.GetComponent<ShopTrigger>().price.ToString();
        ShopPopularitytext.text = "Popularity " + SelectedShop.GetComponent<ShopTrigger>().popularity.ToString() + "%";
        ShopPerPiecetext.text = "Rs " + SelectedShop.GetComponent<ShopTrigger>().price.ToString() + "/piece";
        TotalPieceSold.text = SelectedShop.GetComponent<ShopTrigger>().totalPieceSold.ToString() + " pieces";
        //ShopLeveltext.text = "Level " + SelectedShop.GetComponent<ShopTrigger>().level.ToString(); ;

        LevelShoptext.text = "Level " + SelectedShop.GetComponent<ShopTrigger>().level.ToString();
        //LevelBartext
        int level = SelectedShop.GetComponent<ShopTrigger>().level;
        float upgradePrice = level * 10f;
        Upgradetext.text = "Uprade \nRs." + upgradePrice.ToString();
        float num = ((float)level / 10f);
        UpgradeBar(num);

        EmployeeNumber = SelectedShop.GetComponent<ShopTrigger>().employee;
        UpdateEmployeeImage(EmployeeNumber);
        EmployeeShoptext.text = SelectedShop.GetComponent<ShopTrigger>().employee.ToString();

        DiscountShopext1.text = SelectedShop.GetComponent<ShopTrigger>().discount.ToString() + "%";
        DiscountShopext2.text = SelectedShop.GetComponent<ShopTrigger>().discount.ToString() + "%";

        //Earning Text area
        if (earningshopLoop < 1)
        {
            earningshopLoop = 1;
            StartCoroutine(ShopEarning(SelectedShop));
            
        }
        else
        {
            earningshopLoop = 0;
            StartCoroutine(WaitTOEndLoop());
        }
        UpdateEarning(SelectedShop);


        //show gameobject of upgraded shop
        SelectedShop.GetComponent<ShopTrigger>().UpgradeShopPrefab();

    }

   

    IEnumerator WaitTOEndLoop()
    {
        yield return new WaitForSeconds(1f);
        earningshopLoop = 1;
        StartCoroutine(ShopEarning(SelectedShop));
    }
    IEnumerator ShopEarning(GameObject g)
    {
        while (earningshopLoop > 0)
        {
            UpdateEarning(g);
            yield return new WaitForSeconds(0.05f);
            
        }
    }
    int employeeSalary = 100;
    void UpdateEarning(GameObject g)
    {
        int earn = g.GetComponent<ShopTrigger>().shopEarning;
        EarningText.text = earn.ToString();

        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            if (g.GetComponent<ShopTrigger>().storageName == saveload.shopName[i])
            {
                employeeSalary = saveload.shopEmployeSalary[i];
            }
        }
        int spend = g.GetComponent<ShopTrigger>().employee * employeeSalary * g.GetComponent<ShopTrigger>().employee;       //formula for shopEmployee
        ServiceText.text = spend.ToString();
        TotalText.text = (earn - spend).ToString();                     
        if ((earn - spend) < 0)
        {
            TotalText.color = Color.red;
        }
        else
        {
            TotalText.color = Color.green;
        }
    }

    public void CloseShopUIButton()
    {
        ShopUI.SetActive(false);
        earningshopLoop = 0;
        DisableActivePannel();
    }

    public void ShopUpgradeButton()
    {
        GameObject g = SelectedShop;
        

        int level = g.GetComponent<ShopTrigger>().level;
        float num = ((float)level / 10f);
        if (num != 1)
        {
            g.GetComponent<ShopTrigger>().copyPrice += level*10+level ;
            g.GetComponent<ShopTrigger>().Calculation();
            g.GetComponent<ShopTrigger>().level++;
            level = g.GetComponent<ShopTrigger>().level;
            float upgradePrice = Mathf.Pow(10, level);
            Upgradetext.text = "Uprade \nRs." + upgradePrice.ToString();
            
            saveload.money -= (int)upgradePrice;
            saveload.Save();
        }
        else
        {
            Upgradetext.text = "Max";
        }
        saveload.shopLevel[SelectedShop.GetComponent<ShopTrigger>().shopNumber] = level;
        SelectedShop.GetComponent<ShopTrigger>().Calculation();
        UpdateShopUI(g);
    }

    public void ShopHireButton()
    {
        if (SelectedShop.GetComponent<ShopTrigger>().employee <= 4)
        {
            SelectedShop.GetComponent<ShopTrigger>().employee++;
            EmployeeNumber = SelectedShop.GetComponent<ShopTrigger>().employee;
            SelectedShop.GetComponent<ShopTrigger>().Calculation();
            UpdateShopUI(SelectedShop);
        }
    }

    public void ShopFireButton()
    {
        if (SelectedShop.GetComponent<ShopTrigger>().employee >= 2)
        {
            SelectedShop.GetComponent<ShopTrigger>().employee--;
            EmployeeNumber = SelectedShop.GetComponent<ShopTrigger>().employee;
            SelectedShop.GetComponent<ShopTrigger>().Calculation();
            UpdateShopUI(SelectedShop);
        }
    }

    void UpdateEmployeeImage(int num)
    {
       for (int i = 0; i < 5; i++)
        {
            EmployeeImage[i].sprite = NoEmployeeSprite;
        }

        for (int i = 0; i < num; i++)
        {
            EmployeeImage[i].sprite = EmployeeSprite;
        }

        saveload.shopEmployeeNu[SelectedShop.GetComponent<ShopTrigger>().shopNumber] = num;
        SelectedShop.GetComponent<ShopTrigger>().Calculation();
    }

    public void ShopDiscountButtonPlus()
    {
        if (SelectedShop.GetComponent<ShopTrigger>().discount < 100)
        {
            shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount += 10;
            saveload.shopDiscount[SelectedShop.GetComponent<ShopTrigger>().shopNumber] = shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount;
            SelectedShop.GetComponent<ShopTrigger>().discount = shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount;
            SelectedShop.GetComponent<ShopTrigger>().Calculation();
            UpdateShopUI(SelectedShop);
        }
        else
        {
            //hide or unclickable button
        }
    }

    public void ShopDiscountButtonMinus()
    {
        if (SelectedShop.GetComponent<ShopTrigger>().discount > 0)
        {
            shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount -= 10;
            saveload.shopDiscount[SelectedShop.GetComponent<ShopTrigger>().shopNumber] = shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount;
            SelectedShop.GetComponent<ShopTrigger>().discount = shopData[SelectedShop.GetComponent<ShopTrigger>().shopNumber].ShopDiscount;
            SelectedShop.GetComponent<ShopTrigger>().Calculation();
            UpdateShopUI(SelectedShop);
        }
        else
        {
            //hide or unclickable button
        }
    }

    void UpgradeBar(float num)
    {
        LevelBartext.rectTransform.localScale = new Vector3(num, 1, 1);
    }

    #endregion

    #region CarSpawning

    void CheckForEmptyParkingLot()
    {
        //AllParkingTriggers all the GameObject have script which tell is that parking lot is empty or what
        AllParkingTriggers = GameObject.FindGameObjectsWithTag("ParkingLot");
        lotfound = false;
        foreach (GameObject t in AllParkingTriggers)
        {
            if (t.GetComponent<ParkingLot>().lotBooked == 0)
            {
                lotfound = true;
                break;
            }
        }


        int n = 1;
        if (lotfound)
        {
            while (n > 0)
            {
                int random = UnityEngine.Random.RandomRange(0, AllParkingTriggers.Length);
                GameObject go = AllParkingTriggers[random];
                lotfound = false;
                if (go.GetComponent<ParkingLot>().lotBooked == 0)
                {
                    n = 0;
                    lotfound = true;
                    BookedparkingLotGO = go;
                }

            }
        }
    }

    public void SpawnPeoples(Transform spawnPosition,GameObject myCar)
    {
        float delay = 1;
        float counter = 1;
        int random = UnityEngine.Random.Range(1, 3);
        if(myCar.GetComponent<CarController>()!=null)
        myCar.GetComponent<CarController>().totalPeople = random;
        StartCoroutine(SpawnGameObjectsWithDelay(random,spawnPosition,myCar));
        
    }

    IEnumerator SpawnGameObjectsWithDelay(int random,Transform spawnPosition,GameObject mycar)
    {
        while (random > 0)
        {
            yield return new WaitForSeconds(0.5f);
            int ran = UnityEngine.Random.Range(0, PeoplesGameObject.Length);
            GameObject go = Instantiate(PeoplesGameObject[ran], spawnPosition.position, spawnPosition.rotation);
            go.GetComponent<PeopleController>().myCar = mycar;
            go.GetComponent<PeopleController>().spawnPoint = spawnPosition;
            
            
            random--;
        }

    }

    #endregion

    #region taxi Spawning

    void CheckForTaxiPort()
    {
        if (TaxiPoint.GetComponent<ParkingLot>().lotBooked == 0)
        {
            GameObject go = Instantiate(TaxiGO, CarSpawnPoint.transform.position, CarSpawnPoint.transform.rotation);
            TaxiPoint.GetComponent<ParkingLot>().lotBooked = 1;
        }

        StartCoroutine(WaitForAnotherTaxi());

    }
    IEnumerator WaitForAnotherTaxi()
    {
        int random = UnityEngine.Random.Range(10, 100);
        yield return new WaitForSeconds(random);
        CheckForTaxiPort();
    }

    #endregion

    #region People Spawn At Starting

    [Header("People Random Spawn")]
    public Transform[] PeopleRandomSpawn;
    public Transform OuterPoint1;
    public Transform OuterPoint2;
    public Transform EndPoint;

    IEnumerator SpawnRandomPeopleStarting()
    {
        int PeopleNumber = UnityEngine.Random.Range(4, 10);
        while (PeopleNumber > 0)
        {
            int randomPosition = UnityEngine.Random.Range(0, PeopleRandomSpawn.Length);
            //SpawnPeoples(PeopleRandomSpawn[randomPosition], gameObject);
            int ran = UnityEngine.Random.Range(0, PeoplesGameObject.Length);
            GameObject go = Instantiate(PeoplesGameObject[ran], PeopleRandomSpawn[randomPosition].position, PeopleRandomSpawn[randomPosition].rotation);
            yield return new WaitForSeconds(0.1f);
            go.GetComponent<PeopleController>().ShopSellection();
            go.GetComponent<PeopleController>().checkTravellingPoints();
            go.GetComponent<PeopleController>().spawnPoint = OuterPoint1;
            go.GetComponent<PeopleController>().OuterPoint1 = OuterPoint1;
            go.GetComponent<PeopleController>().OuterPoint2 = OuterPoint2;
            go.GetComponent<PeopleController>().EndPoint = EndPoint;
            yield return new WaitForSeconds(0.1f);
            PeopleNumber--;
        }
    }

    #endregion

    //for UI

    #region trade region

    [Header("ForTradeElements")]
    public GameObject TradeWindow;
    public GameObject TradeBox;
    public Transform TradeBoxContainer;

    private bool tradeWindow = false;
    public void OnTradeButtonClicked()
    {
        if (tradeWindow)
        {
            tradeWindow = false;
            TradeWindow.SetActive(false);
            DisableActivePannel();
        }
        else
        {
            tradeWindow = true;
            TradeWindow.SetActive(true);
            isPannelActive = true;
            DoStuffForTradeWindow();
        }

    }

    List<string> countryName = new List<string>();
    List<string> storageName = new List<string>();
    List<int> totalStock = new List<int>();
    List<bool> tradeDone = new List<bool>();

    public string[] CountryName = new string[] { "India", "Japan", "China", "America", "France", "Rissia" };
    public string[] StorageName = new string[] { "Food", "Mobile", "Sofa", "Jwelery" };

    public Sprite[] storageImage;
    private Sprite storageImageTemp;

    void ClearTradeData()
    {
        countryName.Clear();
        storageName.Clear();
        totalStock.Clear();
        tradeDone.Clear();
    }

    public void  OnTradeCloseButtonPressed()
    {
        isPannelActive = false;
        tradeWindow = false;
        TradeWindow.SetActive(false);
    }

    void DoStuffForTradeWindow()
    {
        //remove previous tardebox
        GameObject[] go = GameObject.FindGameObjectsWithTag("TradeBox");
        foreach (GameObject g in go)
        {
            Destroy(g);
        }
        //RandomEntryForTrade();
        //get the trade elements from save file TODO
        
        //load the other trade files
        if (countryName.Count == 0)
        {
            //do random and store country events
            ClearTradeData();
            RandomEntryForTrade();
        }
        print(countryName.Count+"||"+totalStock.Count);
        //show the elements to user
        
        for(int i=0;i<countryName.Count-1;i++)
        {
            
            GameObject tradeBox = Instantiate(TradeBox);
            tradeBox.transform.SetParent(TradeBoxContainer.transform);
            tradeBox.transform.localScale = Vector3.one;
            tradeBox.transform.Find("CountryName").GetComponent<Text>().text = countryName[i];
            tradeBox.transform.Find("Storage").GetComponent<Text>().text = totalStock[i].ToString();

            //show image
            for (int j = 0; j < StorageName.Length; j++)
            {
                if (StorageName[j] == storageName[i])
                {
                    storageImageTemp = storageImage[j];
                }
            }
            tradeBox.transform.Find("StorageIcon").GetComponent<Image>().sprite = storageImageTemp;

                if (tradeDone[i] == false)
                {
                    //show money
                    tradeBox.transform.Find("Pay").gameObject.SetActive(true);
                    tradeBox.transform.Find("TradeDone").gameObject.SetActive(false);

                    //assign trade function and money
                    tradeBox.transform.Find("Pay").transform.Find("TradeMoney").GetComponent<Text>().text = (totalStock[i]).ToString();

                    
                    int tradenumber=totalStock[i];
                    string tradename = storageName[i];
                    int index = i;
                    int tradePrice = tradenumber * 2;
                    string tradeCOuntry=countryName[i];
                    tradeBox.transform.Find("Pay").GetComponent<Button>().onClick.AddListener(() => OnButtonClick(tradenumber, tradename, index, tradePrice, tradeCOuntry));//(totalStock[i])
                }
                else
                {
                    tradeBox.transform.Find("Pay").gameObject.SetActive(false);
                    tradeBox.transform.Find("TradeDone").gameObject.SetActive(true);
                }
                
        }
    }


    public void OnButtonClick(int tradeAmount,string tradeName,int index,int tradePrice,string tradeCountry)//int moneyTrade,
    {
        isPannelActive = true;
        tradeWindow = true;
        
        //Debug.Log(tradeAmount + "ButtonPressed" + index);
        saveload.money -= tradePrice;
        saveload.Save();
        ShowMoney();
        //TradeWindow.SetActive(false);
        tradeDone[index] = true;
        
        StartCoroutine(CreateShip(tradeAmount, tradeName, index, tradePrice, tradeCountry));
        DoStuffForTradeWindow();
        
    }

    
    void RandomEntryForTrade()
    {
        
        int randomTrade = UnityEngine.Random.Range(3, 10);
        for (int i = 0; i < randomTrade; i++)
        {
            int randomCountryNo = UnityEngine.Random.Range(0, CountryName.Length);
            countryName.Add(CountryName[randomCountryNo]);
            int randomStorageNo = UnityEngine.Random.Range(0, StorageName.Length);
            storageName.Add(StorageName[randomStorageNo]);
            int randomStock = UnityEngine.Random.Range(500, 2000);
            totalStock.Add(randomStock);
            tradeDone.Add(false);
        }
    }

    [Header("ShipElements")]
    public GameObject ShipGO;
    public GameObject Container;

    public Transform ShipSpawnPoint;
    private GameObject[] Ports;
    int nn = 1;
    IEnumerator CreateShip(int tradeAmount, string tradeName, int index,int tradePrice,string tradeCountry)
    {
        nn = 1;
        while (nn > 0)
        {
            Ports = GameObject.FindGameObjectsWithTag("Port");
            //check if storage is empty then go oterwise wait TODO

            int randomPort = UnityEngine.Random.Range(0, Ports.Length);
            if (Ports[randomPort].GetComponent<PortTrigger>().occupied == 0)
            {
                
                Ports[randomPort].GetComponent<PortTrigger>().occupied = 1;
                GameObject go = Instantiate(ShipGO, ShipSpawnPoint.transform.position, ShipSpawnPoint.rotation);
                GameObject goContainer = Instantiate(Container, go.transform.Find("ContainerPosition").transform.position, go.transform.Find("ContainerPosition").transform.rotation);
                //goContainer.GetComponent<ContainerController>().
                //.transform.localScale = Vector3.zero;
                goContainer.GetComponent<Rigidbody>().isKinematic = true;
                goContainer.transform.SetParent(go.transform);
                goContainer.GetComponent<ContainerController>().storageCountry = tradeCountry;
                goContainer.GetComponent<ContainerController>().storageName = tradeName;
                goContainer.GetComponent<ContainerController>().storageQ = tradeAmount;
                goContainer.GetComponent<ContainerController>().tradePrice = tradePrice;
                go.GetComponent<ShipController>().FinalPortStop = Ports[randomPort];
                nn = 0;
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < Ports.Length; i++)
                {
                    if (Ports[randomPort].GetComponent<PortTrigger>().occupied == 0)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    yield return new WaitForSeconds(3f);
                }

             }
        }
        
    }

   
    public void RemoveTrade(string tradeCountry,string tradeStorageName,int storage)
    {
        //print(storageName.Count+"--------------------------------------------------------------------------");
        for (int j = 0; j < storageName.Count; j++)
        {
           // print(storageName[j] + "+" + tradeStorageName + "|" + countryName[j] + "+" + tradeCountry + "|" + totalStock[j] + "+" + storage);
            if (storageName[j] == tradeStorageName && countryName[j] == tradeCountry && totalStock[j] == storage && tradeDone[j] == true)
            {
                storageName.RemoveAt(j);
                countryName.RemoveAt(j);
                totalStock.RemoveAt(j);
                tradeDone.RemoveAt(j);
                
            }
        }
        //print(storageName.Count+"removed");
        DoStuffForTradeWindow();
    }


    public Text TradeCounterTextGlobe;
    void TradeQueueCounter()
    {
        int c = 0;
        for (int i = 0; i < tradeDone.Count; i++)
        {
            if (tradeDone[i] == true)
            {
                c++;
            }
        }


        TradeCounterTextGlobe.text = c.ToString();

        if (c == 0)
            TradeCounterTextGlobe.text = "";
    }

    public Text[] ShipTimersText;
    public void UpdateTimerShips(int index,int min,int sec)
    {
        ShipTimersText[index].text = min.ToString() + ":" + sec.ToString();
        if (sec <= 1 && min < 1)
        {
            ShipTimersText[index].text = "";
        }
    }


    public void TradeRefreshData()
    {
       //clear all other data
        for (int i = 0; i < countryName.Count; i++)
        {
            if (tradeDone[i] == false)
            {
                countryName.RemoveAt(i);
                storageName.RemoveAt(i);
                totalStock.RemoveAt(i);
                tradeDone.RemoveAt(i);
            }
        }
        RandomEntryForTrade();
        DoStuffForTradeWindow();
    }

    #endregion

    public void CheckMyLand()
    {

       // GameObject[] ggg = GameObject.FindGameObjectsWithTag("Shop");
        //foreach (GameObject g in ggg)
        //{
            //Destroy(g);
        //}

        myLand = saveload.landNo;
        string data = saveload.landData;
        string[] items = data.Split(';');
        int len = items.Length;
        int numberOFShops = saveload.totalUnlockShop;
        int d = 0;

        GameObject[] shopspawnPoint = GameObject.FindGameObjectsWithTag("ShopSpawnPoint");//set the shop size equal to it

        for (int i = 0; i < len - 1; i++)
        {

            if (GetDataValue(items[i], "LandName:") == "Land" + myLand.ToString())
            {

                for (int j = 0; j < saveload.shopName.Length; j++)
                {
                    shopData.Add(new ShopData(saveload.shopName[j],
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Quantity:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Discount:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Employee:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Level:"))));

                    //Instantiate all the shops
                    if (d < numberOFShops)
                    {
                        d++;
                        GameObject go = Instantiate(Shop[j], shopspawnPoint[j].transform.position, shopspawnPoint[j].transform.rotation);
                        go.transform.Find("Canvas").transform.position = shopspawnPoint[j].transform.GetChild(0).transform.position;
                        go.transform.Find("Canvas").transform.rotation = shopspawnPoint[j].transform.GetChild(0).transform.rotation;
                        go.name = saveload.shopName[j];
                        GameObject g = go.transform.Find("Shop").gameObject;
                        g.GetComponent<ShopTrigger>().storage = shopData[j].ShopQuantity;
                        g.GetComponent<ShopTrigger>().storageName = shopData[j].ShopName;
                        g.GetComponent<ShopTrigger>().price = saveload.shopPrice[j];
                        g.GetComponent<ShopTrigger>().storage = saveload.shopStorage[j];
                        g.GetComponent<ShopTrigger>().discount = saveload.shopDiscount[j];
                        g.GetComponent<ShopTrigger>().employee = saveload.shopEmployeeNu[j];
                        g.GetComponent<ShopTrigger>().level = saveload.shopLevel[j];

                        
                    }
                }

            }
            else
            {
                //error restart app
            }
        }
    }

    public void AddShop(int shopIndex)
    {
        int j = shopIndex - 1;
        GameObject[] shopspawnPoint = GameObject.FindGameObjectsWithTag("ShopSpawnPoint");

        /*shopData.Add(new ShopData(saveload.shopName[j],
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Quantity:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Discount:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Employee:")),
                    Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Level:"))));
        */

        //Instantiate all the shops
        GameObject go = Instantiate(Shop[j], shopspawnPoint[j].transform.position, shopspawnPoint[j].transform.rotation);
        go.transform.Find("Canvas").transform.position = shopspawnPoint[j].transform.GetChild(0).transform.position;
        go.transform.Find("Canvas").transform.rotation = shopspawnPoint[j].transform.GetChild(0).transform.rotation;
        go.name = saveload.shopName[j];
        GameObject g = go.transform.Find("Shop").gameObject;
        g.GetComponent<ShopTrigger>().storage = shopData[j].ShopQuantity;
        g.GetComponent<ShopTrigger>().storageName = shopData[j].ShopName;
        g.GetComponent<ShopTrigger>().price = saveload.shopPrice[j];
        g.GetComponent<ShopTrigger>().storage = saveload.shopStorage[j];
        g.GetComponent<ShopTrigger>().discount = saveload.shopDiscount[j];
        g.GetComponent<ShopTrigger>().employee = saveload.shopEmployeeNu[j];
        g.GetComponent<ShopTrigger>().level = saveload.shopLevel[j];
        ShowMoney();

    }


    [Header("CameraSwitch")]
    public GameObject mainCamera;
    public GameObject storageCamera;

    bool cameraOnStorage = false;
    public void StorageAndMainCameraSwitch()
    {
        if (!cameraOnStorage)
        {
            //show storage camera
            storageCamera.SetActive(true);
            mainCamera.SetActive(false);
            cameraOnStorage = true;
        }
        else
        {
            //show main camera
            mainCamera.SetActive(true);
            storageCamera.SetActive(false);
            cameraOnStorage = false;
        }
    }

    [Header("MainUI")]
    public Text MoneyText;

    public void ShowMoney()
    {
        MoneyText.text = saveload.money.ToString();
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("?"))
            value = value.Remove(value.IndexOf("?"));
        return value;
    }
}
