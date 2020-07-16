using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTopTrigger : MonoBehaviour
{
    public GameObject Portmanager;
    void OnTriggerEnter(Collider col)
    {
        if (Portmanager.GetComponent<PortManager>().haveContainer == false)
        {
            Portmanager.GetComponent<PortManager>().BaseRight();
        }
        else
        {
            Portmanager.GetComponent<PortManager>().BaseLeft();
        }
    }
}
