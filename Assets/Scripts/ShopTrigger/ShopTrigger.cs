using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class ShopTrigger : MonoBehaviour
{
    public TextMeshProUGUI m_Text_people;
    public TextMeshProUGUI m_Text_storage;
    
    private int peopleInCounter=0;
    public int storage = 0;//all stored from gamecontroller while instantiating
    public int price = 200;//all stored from gamecontroller while instantiating
    public int piece = 2;//all stored from gamecontroller while instantiating
    public string storageName = "Food";//all stored from gamecontroller while instantiating
    public int discount = 0;
    public int level = 1;
    public int employee = 1;
    public int popularity = 1;
    public int totalPieceSold = 0;

    public int shopNumber = 1;

    public GameObject VanSpawnPoint;
    private GameObject gameManager;

    public int shopEarning=0;

    public int copyPrice = 0;

    public GameObject[] ShopUpgrades;

    void Start()
    {
        gameManager=GameObject.FindGameObjectWithTag("GameManager");
        price = 200;
        copyPrice = price;
        
        peopleInCounter = 0;
        GetShopNumber();
        //UpdateStorage();

        for (int i = 0; i < WaitingGO.Length; i++)
        {
            WaitingGO[i].SetActive(false);
        }
        equalDividerQueue = 0;
        UpgradeShopPrefab();
    }

   public void UpgradeShopPrefab()
    {
        for (int j = 0; j < ShopUpgrades.Length; j++)
        {
            ShopUpgrades[j].SetActive(false);
        }

        ShopUpgrades[level - 1].SetActive(true);
    }

    void GetShopNumber()
    {
        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            if (saveload.shopName[i] == storageName)
            {
                storage = saveload.shopStorage[i];
                price = saveload.shopPrice[i];
                copyPrice = price;
                shopNumber = i;
                employee = saveload.shopEmployeeNu[i];
            }
        }
        Calculation();
    }

   

    void OnTriggerEnter(Collider col)
    {
        GameObject TempGo;
        
        col.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        col.gameObject.GetComponent<PeopleController>().enabled = false;
        col.gameObject.SetActive(false);

        peopleInCounter++;
        Calculation();
        TempGo=col.gameObject;


        SetLoadingImage(TempGo);

        //StartCoroutine(WaitTillShopping( TempGo));
    }

    [Header("For Waiting Effect")]
    public GameObject[] WaitingGO;
    public Image[] LoadingImage;
    public TextMeshProUGUI[] PeopleInCounterText;

    private int peopleIn = 0;
    private float loadBar = 0;
    private int TotalEmployeeAtService = 0;
    int[] a = new int[] { 0, 0, 0, 0, 0 };

    private int equalDividerQueue = 0;

    void SetLoadingImage(GameObject temp)
    {
        TotalEmployeeAtService = employee;
        bool flag = false;
        for (int i = 0; i < TotalEmployeeAtService; i++)
        {
            
            if (a[i] == 0)
            {
                a[i]++;
                flag = true; equalDividerQueue++;
                WaitingGO[i].SetActive(true);
                StartCoroutine(WaitLoadingStart(temp, i));
                break;
            }
            
        }
        if (equalDividerQueue > TotalEmployeeAtService-1)
            equalDividerQueue = 0;
        if (flag == false)
        {
            //not got place in queue

            StartCoroutine(WaitToStartLoading(temp, TotalEmployeeAtService));
            TotalEmployeeAtService++;
        }

        //SetTextMeshPro
        for (int i = 0; i < TotalEmployeeAtService; i++)
        {
            PeopleInCounterText[i].text = (a[i]+1).ToString();
            //print(a[i]);
        }
    }

    IEnumerator WaitLoadingStart(GameObject temp,int i)
    {
        int c = 1;
        //print("Index=" + i);
        float EmployeeSpeed = c / 150f;
        bool isLoading = true;
        while (isLoading)
        {
            yield return new WaitForSeconds(0.1f);
            LoadingImage[i].fillAmount=EmployeeSpeed;
            c++;
            EmployeeSpeed = c / 150f;
            if (EmployeeSpeed == 1)
                isLoading = false;

        }
        WaitingGO[i].SetActive(false);
        a[i] -=1;
        PriceDeduct(temp);
    }

    IEnumerator WaitToStartLoading(GameObject temp,int index)
    {
        bool isWaiting = true;
        while (isWaiting)
        {
            yield return new WaitForSeconds(1f);
            if (a[index] == 0)
            {
                isWaiting = false;
                SetLoadingImage(temp);
            }
        }

    }

   
    void PriceDeduct(GameObject temp)
    {
        //waitingtime
        //float time = (saveload.totalEmployeeShop-employee) * 3;//show animation
        
        //yield return new WaitForSeconds(time);

        storage -= piece;
        totalPieceSold += piece;
        saveload.money += price;
        shopEarning+=price;
        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            if (saveload.shopName[i] == storageName)
            {
                saveload.shopStorage[i] = storage;
             }
        }

        
        temp.GetComponent<PeopleController>().money -= price;

        temp.SetActive(true);
        peopleInCounter--;
        SetTextMeshPro();
        temp.GetComponent<CapsuleCollider>().enabled = false;
        temp.GetComponent<NavMeshAgent>().enabled = true;
        temp.GetComponent<PeopleController>().enabled = true;
        temp.GetComponent<PeopleController>().ShopSellection();
        StartCoroutine(EnableCollidersOfPeoples(temp));

        //update the  effect in the storage area
        gameManager.GetComponent<StorageController>().LoadAndUpdateStorage();                                                                                                                   
    }

    IEnumerator EnableCollidersOfPeoples(GameObject temp)
    {
        yield return new WaitForSeconds(3f);
        temp.GetComponent<CapsuleCollider>().enabled = true;

        //give money and effect
    }

    public void Calculation()
    {
        popularity = (level * 10) + discount;
        piece = Random.Range(1, (discount + 1) + (popularity / 10));
        price = copyPrice - ((copyPrice * discount) / 100);
        SetTextMeshPro();
    }

    void SetTextMeshPro()
    {
        m_Text_people.text = peopleInCounter.ToString();
        m_Text_storage.text = storage.ToString();
        gameManager.GetComponent<GameController>().ShowMoney();
    }

    public void UpdateStorage()
    {
        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            if (saveload.shopName[i] == storageName)
            {
                storage = saveload.shopStorage[i];
                price = saveload.shopPrice[i];
            }
        }
        Calculation();
    }

    
}
//private TextContainer m_TextContainer;
