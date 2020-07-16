using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickContainer : MonoBehaviour
{
    public GameObject PortManager;
    void OnTriggerEnter(Collider col)
    {
        print(col.gameObject.name);
        if (col.gameObject.tag == "Container")
        {
            
            col.gameObject.transform.SetParent(gameObject.transform);
            PortManager.GetComponent<PortManager>().haveContainer = true;

            PortManager.GetComponent<PortManager>().HandleUp();
        }
    }
}
