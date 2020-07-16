using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkingUpgrade : MonoBehaviour
{
    public GameObject ParkingPannel;

    public Text ParkingSpaceText;
    public Text LevelText;
    public Text EarningText;

    public Text UpgradeButtonText;
    public Image UpgradeBar;

    private int UpgradeCost=0;

    public GameObject[] Level1;
    public GameObject[] Level2;
    public GameObject[] Level3;
    public GameObject[] Level4;
    public GameObject[] Level5;
    public GameObject[] Level6;

    

    void Awake()
    {
        Show();
        ParkingPannel.SetActive(false);
        SetText();
        
    }

    public void OpenParkingPannel()
    {
        ParkingPannel.SetActive(true);
        SetText();
    }

    public void OnCloseButton()
    {
        ParkingPannel.SetActive(false);
        gameObject.GetComponent<GameController>().DisableActivePannel();
    }

    void SetText()
    {
        GameObject[] g = GameObject.FindGameObjectsWithTag("ParkingLot");
        ParkingSpaceText.text = "Parking Space " + g.Length.ToString();
        LevelText.text = "Level " + saveload.parkingLevel.ToString();
        saveload.parkingEarning = g.Length * saveload.parkingLevel * 100;   //formula for calculating parking earning
        EarningText.text = saveload.parkingEarning.ToString();
        UpgradeCost = (int)Mathf.Pow(50, saveload.parkingLevel);               //formula for calculating parking earning
        UpgradeButtonText.text = "Upgrade\nRs."+UpgradeCost.ToString();
        float num = (int)saveload.parkingLevel / 6f;                        //formula for calculating parking earning
        UpgradeBar.rectTransform.localScale = new Vector3(num, 1, 1);
        
        if (saveload.parkingLevel == 6)
        {
            UpgradeButtonText.text = "Max";
        }
    }

    public void UpgradeButtonPressed()
    {
        if (UpgradeCost <= saveload.money && saveload.parkingLevel<6)
        {
            saveload.money -= UpgradeCost;
            saveload.parkingLevel++;
            SetText();
            gameObject.GetComponent<GameController>().ShowMoney();
        }
        else
        {
            //show not enough money
        }

        Show();
    }

    void Show()
    {
        //clear
        foreach (GameObject g in Level1)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Level2)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Level3)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Level4)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Level5)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in Level6)
        {
            g.SetActive(false);
        }


        //show

        if (saveload.parkingLevel == 1)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }

        }
        else if (saveload.parkingLevel == 2)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level2)
            {
                g.SetActive(true);
            }
        }
        else if (saveload.parkingLevel == 3)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level2)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level3)
            {
                g.SetActive(true);
            }
        }
        else if (saveload.parkingLevel == 4)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level2)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level3)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level4)
            {
                g.SetActive(true);
            }
        }
        else if (saveload.parkingLevel == 5)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level2)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level3)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level4)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level5)
            {
                g.SetActive(true);
            }
        }
        else if (saveload.parkingLevel == 6)
        {
            foreach (GameObject g in Level1)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level2)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level3)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level4)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level5)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in Level6)
            {
                g.SetActive(true);
            }
        }
    }
}
