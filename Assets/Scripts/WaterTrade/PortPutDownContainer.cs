using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortPutDownContainer : MonoBehaviour
{
    public GameObject PortManager;

    void OnTriggerEnter(Collider col)
    {
        PortManager.GetComponent<PortManager>().HandleDown();
    }
}
