using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanDestroyer : MonoBehaviour
{
   void  OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Van")
        {
            saveload.totalVansOut++;
            Destroy(col.gameObject);
        }
    }
}
