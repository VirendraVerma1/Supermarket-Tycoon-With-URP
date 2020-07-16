using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContainerController : MonoBehaviour
{
    public TextMeshProUGUI storagetext;

    public string storageCountry;
    public string storageName;
    public int storageQ;
    public int tradePrice;

    public GameObject Canvas;

    void Start()
    {
        storagetext.text = storageQ.ToString();
    }

    public void Update()
    {
        Canvas.transform.LookAt(Camera.main.transform);
    }

    public void SetText(int storage)
    {
        storagetext.text = storage.ToString();
    }
}
