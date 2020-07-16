using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortOffice : MonoBehaviour
{
    public GameObject PortOfficeGO;

    public Text ServiceVanText;
    public Text PortNoText;

    public Text ServiceChargeText;

    public Text EmployeeText;

    public Sprite EmployeeSprite;
    public Sprite NoEmployeeSprite;

    public Image[] EmployeeImage;

    public GameObject[] PortButtons;
    public GameObject[] PortUpgradeBar;
    public Image[] PortUpgradeProgress;
    public GameObject[] Ports;
    public Text[] PortNamesLevel;
    public Button[] UpgradeButton;

    // Start is called before the first frame update
    void Start()
    {
        PortOfficeGO.SetActive(false);
        SetTextData();
    }

    public void OpenPortOfficePannel()
    {
        PortOfficeGO.SetActive(true);
        SetTextData();
    }

    public void OnCloseButtonPressed()
    {
        PortOfficeGO.SetActive(false);
        gameObject.GetComponent<GameController>().DisableActivePannel();
    }

    void SetTextData()
    {
        ServiceVanText.text = "Service Van " + saveload.mytotalvan.ToString();
        PortNoText.text = "Ports " + saveload.myports.ToString();
        saveload.serviceCharge = saveload.myports * saveload.mytotalvan * 1000;  //formula for calculating port service charge
        ServiceChargeText.text = saveload.serviceCharge.ToString();
        EmployeeText.text = saveload.mytotalvan.ToString();
        UpdateEmployeeImage(saveload.mytotalvan);
        ShowPortButtons();
        ShowPorts();
        gameObject.GetComponent<GameController>().ShowMoney();
        PortNamesAndLevelUpdate();
    }

    void PortNamesAndLevelUpdate()
    {
        //PortNamesLevel
       
        for (int i = 0; i < saveload.myports; i++)
        {
            PortNamesLevel[i].text = "Port "+(i + 1).ToString()+" (Level"+Ports[i].GetComponent<PortManager>().level+")";
            PortUpgradeProgress[i].rectTransform.localScale = new Vector3(Ports[i].GetComponent<PortManager>().level/10f, 1, 1);
        }
    }

    void ShowPortButtons()
    {
        for (int i = 0; i < PortButtons.Length; i++)
        {
            PortButtons[i].SetActive(true);
            PortUpgradeBar[i].SetActive(false);
            
        }

        for (int i = 0; i < saveload.myports; i++)
        {
            PortButtons[i].SetActive(false);
            PortUpgradeBar[i].SetActive(true);
            UpgradeButton[i].GetComponentInChildren<Text>().text = "Upgrade\n" + "Rs." + Ports[i].GetComponent<PortManager>().UpgradeCost;
        }
    }

    void ShowPorts()
    {
        for (int i = 0; i < saveload.myports; i++)
        {
            Ports[i].SetActive(true);
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


    }
 
    public void HireButtonPressed()
    {
        if (saveload.mytotalvan <= 4)
        {
            saveload.mytotalvan++;
            saveload.totalVansOut++;
            
            SetTextData();
            gameObject.GetComponent<StorageController>().CheckForAnotherStorage();
        }
        print("Van" + saveload.totalVansOut);
    }

    public void FireButtonPressed()
    {
        if (saveload.mytotalvan >= 2)
        {
            saveload.mytotalvan--;
            saveload.totalVansOut--;
            SetTextData();

            if (saveload.totalVansOut < saveload.mytotalvan)
            {
                gameObject.GetComponent<StorageController>().RemoveVan();
                saveload.totalVansOut++;
            }
            
            
        }
        print("Van1" + saveload.mytotalvan);
        print("Van" + saveload.totalVansOut);
    }

    public void Port1Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.myports = 1;
            SetTextData();
         }
    }
    public void Port2Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.myports = 2;
            SetTextData();
        }
    }
    public void Port3Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.myports = 3;
            SetTextData();
        }
    }
    public void Port4Button(int cost)
    {
        if (saveload.money >= cost)
        {
            saveload.money -= cost;
            saveload.myports = 4;
            SetTextData();
        }
    }

    public void OnUpgradeButtonPressed(int index)
    {
        if (saveload.money >= Ports[index].GetComponent<PortManager>().UpgradeCost)
        {
            saveload.money -= Ports[index].GetComponent<PortManager>().UpgradeCost;
            Ports[index].GetComponent<PortManager>().level++;
            Ports[index].GetComponent<PortManager>().LevelUpdates();
            SetTextData();
        }
    }
}
