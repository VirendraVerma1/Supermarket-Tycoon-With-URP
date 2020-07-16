using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParkingLot : MonoBehaviour
{
    public int lotBooked;
    public GameObject[] peoples;

    public Transform peopleSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        //take the value from the savefile default will be 0
        lotBooked = 0; //0 means no car 1 means lot  is booked but no car 2 means lot is booked with car
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<CarController>()!=null)
            col.gameObject.GetComponent<CarController>().enabled = false;
        else
            col.gameObject.GetComponent<TaxiController>().enabled = false;
        col.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        col.gameObject.transform.rotation = gameObject.transform.rotation;
        lotBooked = 2;
        col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //call the spawn people function on the gamecontroller
        GameObject.Find("GameManager").GetComponent<GameController>().SpawnPeoples(peopleSpawnPosition,col.gameObject);

        if (col.gameObject.GetComponent<CarController>() == null)
        {
            StartCoroutine(EnableTaxiComponents(col.gameObject));
        }
    }

    IEnumerator EnableTaxiComponents(GameObject go)
    {
        
        yield return new WaitForSeconds(3f);
        go.GetComponent<BoxCollider>().enabled = false;
        go.GetComponent<NavMeshAgent>().enabled = true;
        go.GetComponent<Rigidbody>().isKinematic = false;
        go.gameObject.GetComponent<TaxiController>().enabled = true;
        go.gameObject.GetComponent<TaxiController>().ReactivateNavAgent();
        lotBooked = 0;
        StartCoroutine(ActivateBoxCollider(go));
    }

    IEnumerator ActivateBoxCollider(GameObject go)
    {
        yield return new WaitForSeconds(2f);
        go.GetComponent<BoxCollider>().enabled = true;
    }
}
