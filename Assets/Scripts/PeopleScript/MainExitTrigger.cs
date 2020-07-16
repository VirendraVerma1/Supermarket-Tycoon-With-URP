using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainExitTrigger : MonoBehaviour
{
    private GameObject[] lastQueueCashier;
    

    void Start()
    {
        lastQueueCashier = GameObject.FindGameObjectsWithTag("LastQueue");
        
    }

    void OnTriggerEnter(Collider col)
    {

        StartCoroutine(waitToGoForQueue(col));
    }

    IEnumerator waitToGoForQueue(Collider col)
    {
        if (!col.gameObject.GetComponent<PeopleController>().flagGotCashier)
        {
            yield return new WaitForSeconds(0.5f);
            CheckCashier(col.gameObject);
        }
    }

    void CheckCashier(GameObject go)
    {
        for (int i = 0; i < lastQueueCashier.Length; i++)
        {
            if (lastQueueCashier[i].gameObject.GetComponent<CashierQueue>().space == 0)
            {
                go.GetComponent<PeopleController>().flagGotCashier = true;
                lastQueueCashier[i].gameObject.GetComponent<CashierQueue>().space = 1;
                go.GetComponent<PeopleController>().QueueCashierSet(lastQueueCashier[i].transform);
                break;
            }
        }
    }
}
