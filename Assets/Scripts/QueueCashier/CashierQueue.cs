using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierQueue : MonoBehaviour
{
    public GameObject myNextLink;
    public int space = 0;

    

    void OnTriggerEnter(Collider col)
    {
        if (gameObject.tag == "StartQueue")
        {

            StartCoroutine(WaitForCashCut(col));
        }else{
            if (myNextLink.GetComponent<CashierQueue>().space == 0)
            {

                if (myNextLink.tag != "StartQueue")
                {
                    myNextLink.GetComponent<CashierQueue>().space = 1;
                }
                
                col.gameObject.GetComponent<PeopleController>().QueueCashierSet(myNextLink.transform);
                space = 0;
            }
        }
        

    }
    IEnumerator WaitForCashCut(Collider col)
    {
        
        //col.gameObject.GetComponent<PeopleController>().QueueCashierSet(myNextLink.transform);
        
        col.gameObject.GetComponent<PeopleController>().DoCashCut(gameObject);
        yield return new WaitForSeconds(0.1f);
        
    }

    void OnTriggerExit(Collider col)
    {
        space = 0;
    }
}
