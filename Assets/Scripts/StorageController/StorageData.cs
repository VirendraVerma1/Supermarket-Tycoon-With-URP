using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageData
{
    public string StorageName;
    public int StorageQuantity;
    public int StorageMaxCapacity;

    public StorageData(string storageName, int storageQuantity, int storageMaxCapacity)
    {
        StorageName = storageName;
        StorageQuantity = storageQuantity;
        StorageMaxCapacity = storageMaxCapacity;
    }
}
