using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cranestop : MonoBehaviour
{
    public GameObject PortManager;
    void OnTriggerEnter(Collider col)
    {
        if (PortManager.GetComponent<PortManager>().haveContainer == false)
        {
            
            PortManager.GetComponent<PortManager>().Stop();
            PortManager.GetComponent<PortManager>().ActivateShip();
        }
    }
}
