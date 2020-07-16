using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUpdate : MonoBehaviour
{
    public GameObject StoragePannel;

    public Text TotalStorageText;

    public Text[] StorageCapacity;

    public GameObject[] StorageArea;
    public GameObject[] BuyButton;
    public GameObject[] UpgradeButton;

    public int PortEmployeeService = 0;
    private int n = 0;
    // Start is called before the first frame update
    void Awake()
    {
        n = 0;
        StoragePannel.SetActive(false);
        SetThings();
    }


    public void OnStoragePannelOpen()
    {
        StoragePannel.SetActive(true);
        SetThings();
    }

    public void OnCloseButton()
    {
        StoragePannel.SetActive(false);
        gameObject.GetComponent<GameController>().DisableActivePannel();
    }

    public void OnManagementButtonPressed()
    {
        gameObject.GetComponent<StorageMangement>().OpenStorageManager();
    }

    void SetThings()
    {
        
        CalculateTotalStorage();
        TotalStorageText.text = "Total Storage " + saveload.occupiedStorage+"/"+saveload.totalStorage.ToString();
        ShowStorageArea();
        ShowBuyButtons();
        ShowUpgradeButton();
        SetAllStorageCapacity();
        SetUpgradeCost();
        gameObject.GetComponent<GameController>().ShowMoney();
        if (n == 1)
        {
            gameObject.GetComponent<StorageController>().LoadAndUpdateStorage();
        }


        n = 1;
    }

    void ShowStorageArea()
    {
        for (int i = 0; i < saveload.storageOpen; i++)
        {
            StorageArea[i].SetActive(true);
        }
    }

    void ShowBuyButtons()
    {
        for (int i = 0; i < BuyButton.Length; i++)
        {
            BuyButton[i].SetActive(true);
        }
        for (int i = 0; i < saveload.storageOpen; i++)
        {
            BuyButton[i].SetActive(false);
        }
    }

    void ShowUpgradeButton()
    {
        for (int i = 0; i < UpgradeButton.Length; i++)
        {
            UpgradeButton[i].SetActive(false);
        }

        for (int i = 0; i < saveload.storageOpen; i++)
        {
            UpgradeButton[i].SetActive(true);
        }
    }

    void SetAllStorageCapacity()
    {
        for (int i = 0; i < saveload.storageOpen; i++)
        {
            StorageCapacity[i].text = "Capacity "+saveload.storageCapacity[i].ToString();
        }
    }

    void CalculateTotalStorage()
    {
        saveload.totalStorage=0;

        for (int i = 0; i < saveload.storageCapacity.Length; i++)
        {
            saveload.totalStorage += saveload.storageCapacity[i];
        }
    }

    void SetUpgradeCost()
    {
        for (int i = 0; i < saveload.storageOpen; i++)
        {
            if (saveload.storageCapacity[i] < 60000)
            {
                UpgradeButton[i].GetComponentInChildren<Text>().text = "Upgrade\nRs." + (saveload.storageCapacity[i] * 2).ToString();         //formula for storage upgradation
            }
            else
            {
                UpgradeButton[i].SetActive(false);
            }
        }
    }

    public void BuyStorage1Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[0] += 600;
            SetThings();
        }
    }

    public void BuyStorage2Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[1] += 600;
            SetThings();
        }
    }

    public void BuyStorage3Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[2] += 600;
            SetThings();
        }
    }

    public void BuyStorage4Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[3] += 600;
            SetThings();
        }
    }

    public void BuyStorage5Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[4] += 600;
            SetThings();
        }
    }

    public void BuyStorage6Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.storageOpen++;
            saveload.storageCapacity[5] += 600;
            SetThings();
        }
    }

    public void UpgradeButtonPressed(int index)
    {
        if (saveload.money >= (saveload.storageCapacity[index] * 2))
        {
            saveload.money -= saveload.storageCapacity[index] * 2;
            saveload.storageCapacity[index] += 600;
            SetThings();
        }
    }


    

}
