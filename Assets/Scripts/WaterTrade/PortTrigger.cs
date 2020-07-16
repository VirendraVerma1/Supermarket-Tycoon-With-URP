using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PortTrigger : MonoBehaviour
{
    public GameObject Portmanager;

    public int occupied = 0;

    void OnTriggerEnter(Collider col)
    {
        
        col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        col.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        col.gameObject.GetComponent<ShipController>().enabled = false;
        Portmanager.GetComponent<PortManager>().haveContainer = true;
        Portmanager.GetComponent<PortManager>().HandleDown();
        Portmanager.GetComponent<PortManager>().MyShip = col.gameObject;
    }
}
