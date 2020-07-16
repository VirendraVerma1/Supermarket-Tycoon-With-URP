using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageMangement : MonoBehaviour
{

    public GameObject StorageManagerPannel;

    public Text MaxStorageText;
    public Text FreeStorageText;

    public GameObject[] ShopPannels;
    public Text[] ShopCapacityText;
    public Text[] SHopCapacityText2;
    public GameObject[] LeftButton;
    public GameObject[] RightButton;

    private int totalfreeSpace = 0;

    void Start()
    {
        StorageManagerPannel.SetActive(false);
    }

    public void OpenStorageManager()
    {
        StorageManagerPannel.SetActive(true);
        SetThings();
    }

    public void OnCloseButtonPressed()
    {
        StorageManagerPannel.SetActive(false);
        gameObject.GetComponent<GameController>().DisableActivePannel();
    }

    void SetThings()
    {
        MaxStorageText.text = "Max Storage Limit " + saveload.totalStorage.ToString();

        CalculateFreeStorage();
        FreeStorageText.text = "Free Space " + totalfreeSpace.ToString();
        if (totalfreeSpace == 0)
            FreeStorageText.color = Color.black;
        else
            FreeStorageText.color = Color.green;

        ActivateShopObjectsOnly();
        ActiveThings();

        gameObject.GetComponent<StorageController>().LoadAndUpdateStorage();
    }

    void CalculateFreeStorage()
    {
        totalfreeSpace=0;
        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            totalfreeSpace += saveload.shopMaxStorage[i];
        }
        totalfreeSpace = saveload.totalStorage - totalfreeSpace;
    }

    void ActivateShopObjectsOnly()
    {
        for(int i=0;i<saveload.totalUnlockShop;i++)
        {
            ShopPannels[i].SetActive(true);
        }
    }

    void ActiveThings()
    {
        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            ShopCapacityText[i].text = "Capacity " + saveload.shopStorage[i].ToString() +"/"+ saveload.shopMaxStorage[i].ToString();
            SHopCapacityText2[i].text = saveload.shopMaxStorage[i].ToString();
            if (saveload.shopMaxStorage[i] <= 0)
            {
                LeftButton[i].SetActive(false);
            }
            else
            {
                LeftButton[i].SetActive(true);
            }
            if (totalfreeSpace <= 0)
            {
                RightButton[i].SetActive(false);
            }
            else
            {
                RightButton[i].SetActive(true);
            }
        }
    }

    public void LeftButtonPressed(int index)
    {
        saveload.shopMaxStorage[index] -= 600;
        SetThings();
    }

    public void RightButtonPressed(int index)
    {
        saveload.shopMaxStorage[index] += 600;
        SetThings();
    }
}
