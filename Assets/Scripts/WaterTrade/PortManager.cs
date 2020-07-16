using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PortManager : MonoBehaviour
{
    public GameObject Handle;
    public GameObject Base;
    public GameObject MyShip;
    public Transform CheckShip;

    float handleMovement=0;
    float baseMovement = 0;

    bool handleMovementCon = false;
    bool baseMovementCon = false;

    public float speed = 0.01f;

    public bool haveContainer = false;

    float activatetimer = 10f;
    bool activateShip = false;

    public GameObject[] PortUpgrades;
    public int level = 1;
    public int UpgradeCost=0;
    void Start()
    {
        activatetimer = 10f;
        speed = 0.01f;
        handleMovement = 0;
        baseMovement = 0;
        handleMovementCon = false;
        baseMovementCon = false;
        haveContainer = false;
        

        //TODO save and get the level of port
        LevelUpdates();
    }

    public void LevelUpdates()
    {
        //for gameObjects
        /*
        for (int j = 0; j < PortUpgrades.Length; j++)
        {
            PortUpgrades[j].SetActive(false);
        }

        PortUpgrades[level - 1].SetActive(true);
         */

        //for speed up
        speed = 0.01f * level;
        UpgradeCost = (int)Mathf.Pow(4, level);
    }

    void Update()
    {
        if (handleMovementCon)
        {
            Handle.transform.Translate(0, handleMovement, 0);
        }

        if (baseMovementCon)
        {
            Base.transform.Translate(baseMovement, 0, 0);
        }

        if (activateShip)
        {
            activatetimer -= Time.deltaTime;
            MyShip.transform.Translate(0, 0, -0.1f);
            if (activatetimer < 0)
            {
                activateShip = false;
                ActivateS();
            }
        }
    }

    public void HandleDown()
    {
        handleMovementCon = true;
        baseMovementCon = false;
        handleMovement = -speed;
    }
    public void HandleUp()
    {
        handleMovementCon = true;
        baseMovementCon = false;
        handleMovement = speed;
    }
    public void BaseLeft()
    {
        baseMovementCon = true;
        baseMovement = -speed;
        handleMovementCon = false;
    }
    public void BaseRight()
    {
        baseMovementCon = true;
        handleMovementCon = false;
        baseMovement = speed;
    }
    public void Stop()
    {
        baseMovementCon = false;
        handleMovementCon = false;
       
    }

    public void ActivateShip()
    {
        //check if ship is there from raycast
        
        RaycastHit hit;

        if (Physics.Raycast(CheckShip.position, -Vector3.up, out hit))
        {
            if (hit.collider.gameObject.name == "ShipPath")
            {
                activateShip = false;
               
            }
            else
            {
                //print(hit.collider.gameObject.name);
                activateShip = true;
            }

        }
    }

    void ActivateS()
    {
        activatetimer = 10f;
        MyShip.GetComponent<ShipController>().enabled = true;
        MyShip.GetComponent<NavMeshAgent>().enabled = true;
        MyShip.GetComponent<Rigidbody>().isKinematic = false;
        MyShip.GetComponent<ShipController>().stage = 2;
    }

    
    
}
