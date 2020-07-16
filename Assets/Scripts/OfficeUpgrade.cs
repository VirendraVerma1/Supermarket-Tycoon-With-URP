using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficeUpgrade : MonoBehaviour
{
    public GameObject OfficeCanvas;

    public Text TotalShoptext;
    public Text[] ShopNameText;
    public GameObject[] ShopBuyButton;

    void Start()
    {
        OfficeCanvas.SetActive(false);
    }

    public void OpenOfficePannel()
    {
        OfficeCanvas.SetActive(true);
        CheckIfHaveShopandShow();
    }

    public void CloseOfficePannelButton()
    {
        OfficeCanvas.SetActive(false);
        gameObject.GetComponent<GameController>().DisableActivePannel();
    }

    void CheckIfHaveShopandShow()
    {
        print(saveload.totalUnlockShop);
        //showing only not but buttons
        for (int i = 0; i < saveload.shopName.Length; i++)
        {
              ShopBuyButton[i].SetActive(true);
        }

        for (int i = 0; i < saveload.totalUnlockShop; i++)
        {
            
                ShopBuyButton[i].SetActive(false);
            
        }

    }

    public void Shop1Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.totalUnlockShop = 1;
            CheckIfHaveShopandShow();
            gameObject.GetComponent<GameController>().AddShop(1);
        }
    }

    public void Shop2Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.totalUnlockShop = 2;
            CheckIfHaveShopandShow();
            gameObject.GetComponent<GameController>().AddShop(2);
        }
    }

    public void Shop3Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.totalUnlockShop = 3;
            CheckIfHaveShopandShow();
            gameObject.GetComponent<GameController>().AddShop(3);
        }
    }

    public void Shop4Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.totalUnlockShop = 4;
            CheckIfHaveShopandShow();
            gameObject.GetComponent<GameController>().AddShop(4);
        }
    }

    
}
