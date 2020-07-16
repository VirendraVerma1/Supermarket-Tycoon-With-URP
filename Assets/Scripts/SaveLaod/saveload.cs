using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;


public class saveload : MonoBehaviour
{

    public static int totalUnlockShop = 9; //change
    public static string[] shopName = new string[] { "Food", "Jwelery", "Mobile", "Sofa", "Car", "Toys", "Electronics", "Bottle", "Bike" };//increase landdata
    public static int[] shopStorage = new int[] {600,600,600,600,600,600,600,600,600 };
    public static int[] shopMaxStorage = new int[] { 600, 600, 600, 600,600,600,600,600,600 };
    public static int[] shopPrice = new int[] { 50, 100, 500, 1000,2000,4000,8000,16000,24000 };//increase landdata
    public static int[] shopEmployeSalary = new int[] { 100, 200, 400, 800,1600,3200,6400,12800,25600 };
    public static int[] shopDiscount = new int[] { 0, 0, 0, 0,0,0,0,0,0};//change
    public static int[] shopEmployeeNu = new int[] { 1, 1, 1, 1,1,1,1,1,1 };//change
    public static int[] shopLevel = new int[] { 1, 1, 1, 1,1,1,1,1,1 };//change
    public static int numberOfLands = 4;//-1 for loop
    public static int landNo = 1;//change save in another file
    public static int totalEmployeeShop = 5;
    public static int vanCapacity = 500;//change

    public static int parkingLevel = 1;//change
    public static int parkingEarning = 1; //formula for calculating parking earning is in parking upgrade script

    public static int serviceVanMax=5;
    public static int mytotalvan = 2;//change
    public static int totalVansOut = 2;
    public static int myports = 1;//change
    public static int serviceCharge = 1000;  //formula for calculating port service charge is in port office script

    public static int totalStorage = 5000;//change
    public static int occupiedStorage = 0;//change
    public static int storageOpen = 1;//change
    public static int[] storageCapacity = new int[] { 600, 600, 600, 600, 600, 600 };  //formula for storage upgradation is in storageUpdate script


    public static string globalDateTime=" ";

    public static string accountID = " ";
    public static string playerName = " ";
    public static int money =1000000;
    public static string landData = " ";
    

    public static string current_filename = "info.dat";

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + current_filename);
        Notebook_Data data = new Notebook_Data();

        CreateLandData();

        data.AccountID = accountID;
        data.PlayerName = Encrypt(playerName);
        data.Money = money;
        data.LandData = landData;
        //data.Money = int.Parse(Encrypt(money.ToString()));
        //data.Money = Convert.ToInt32( Encrypt(money.ToString()));

        bf.Serialize(file, data);
        file.Close();
        
    }

    public static void Load()
    {

        if (File.Exists(Application.persistentDataPath + "/" + current_filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + current_filename, FileMode.Open);/* */
            Notebook_Data data = (Notebook_Data)bf.Deserialize(file);

            accountID=data.AccountID;
            playerName=Decrypt(data.PlayerName);
            money = data.Money;
            landData = data.LandData;
            //money = int.Parse(Decrypt(data.Money.ToString()));
            //print(landData);
            file.Close();
            LoadToArray();
        }
        else
        {
            current_filename = "info.dat";
            accountID = " ";
            
            //create dynamically through coding from shop data
            CreateLandData();
            //landData = "LandName:Land1?Land1:1?Shop1:1?Shop1Quantity:1000?Shop1Discount:0?Shop1Employee:1?Shop1Level:1?Shop2:2?Shop2Quantity:1000?Shop2Discount:0?Shop2Employee:1?Shop2Level:1?Shop3:3?Shop3Quantity:1000?Shop3Discount:0?Shop3Employee:1?Shop3Level:1?Shop4:4?Shop4Quantity:1000?Shop4Discount:0?Shop4Employee:1?Shop4Level:1;LandName:Land2?Land2:0?Shop1:1?Shop1Quantity:1000?Shop1Discount:0?Shop1Employee:1?Shop1Level:1?Shop2:2?Shop2Quantity:1000?Shop2Discount:0?Shop2Employee:1?Shop2Level:1?Shop3:3?Shop3Quantity:1000?Shop3Discount:0?Shop3Employee:1?Shop3Level:1?Shop4:4?Shop4Quantity:1000?Shop4Discount:0?Shop4Employee:1?Shop4Level:1;LandName:Land3?Land3:0?Shop1:1?Shop1Quantity:1000?Shop1Discount:0?Shop1Employee:1?Shop1Level:1?Shop2:2?Shop2Quantity:1000?Shop2Discount:0?Shop2Employee:1?Shop2Level:1?Shop3:3?Shop3Quantity:1000?Shop3Discount:0?Shop3Employee:1?Shop3Level:1?Shop4:4?Shop4Quantity:1000?Shop4Discount:0?Shop4Employee:1?Shop4Level:1;LandName:Land4?Land4:0?Shop1:1?Shop1Quantity:1000?Shop1Discount:0?Shop1Employee:1?Shop1Level:1?Shop2:2?Shop2Quantity:1000?Shop2Discount:0?Shop2Employee:1?Shop2Level:1?Shop3:3?Shop3Quantity:1000?Shop3Discount:0?Shop3Employee:1?Shop3Level:1?Shop4:4?Shop4Quantity:1000?Shop4Discount:0?Shop4Employee:1?Shop4Level:1;";
            saveload.Save();

        }
    }

    private static void CreateLandData()
    {
        string data="";
        for (int i = 0; i < numberOfLands; i++)
        {
            data += "LandName:Land" + (i + 1).ToString() + "?Land" + (i + 1).ToString() + ":" + (i + 1).ToString() + "?" + "LandUnlockedShop:" + totalUnlockShop + "?LandVanCapacity:" + vanCapacity + "?MyTotalVan:" + mytotalvan + "?ParkingLevel:" + parkingLevel + "?Ports:" + myports + "?TotalStorage:" + totalStorage + "?OccupiedStorage:" + occupiedStorage + "?StorageOpen:" + storageOpen+"?";
            for (int j = 0; j < shopName.Length; j++)
            {
                data += "Shop" + (j + 1).ToString() + ":" + (j + 1).ToString() + "?Shop" + (j + 1).ToString() + "Quantity:" + shopStorage[j] + "?Shop" + (j + 1) + "Discount:" + shopDiscount[j] + "?Shop" + (j + 1) + "Employee:" + shopEmployeeNu[j] + "?Shop" + (j + 1) + "Level:" + shopLevel[j] + "?";
            }
            data += ";";
        }
        
        //print(data);
        landData = data;
    }

    public static void LoadToArray()
    {
        string data = landData;
        string[] items = data.Split(';');
        int len = items.Length;

        for (int i = 0; i < len - 1; i++)
        {

            if (GetDataValue(items[i], "LandName:") == "Land" + landNo)
            {
                totalUnlockShop = Convert.ToInt32(GetDataValue(items[i], "LandUnlockedShop:"));
                vanCapacity = Convert.ToInt32(GetDataValue(items[i], "LandVanCapacity:"));
                parkingLevel = Convert.ToInt32(GetDataValue(items[i], "ParkingLevel:"));
                mytotalvan = Convert.ToInt32(GetDataValue(items[i], "MyTotalVan:"));
                myports = Convert.ToInt32(GetDataValue(items[i], "Ports:"));
                totalStorage = Convert.ToInt32(GetDataValue(items[i], "TotalStorage:"));
                occupiedStorage = Convert.ToInt32(GetDataValue(items[i], "OccupiedStorage:"));
                storageOpen = Convert.ToInt32(GetDataValue(items[i], "StorageOpen:"));


                for (int j = 0; j < saveload.shopName.Length; j++)
                {
                    shopStorage[j] = Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Quantity:"));
                    shopDiscount[j] = Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Discount:"));
                    shopEmployeeNu[j] = Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Employee:"));
                    shopLevel[j] = Convert.ToInt32(GetDataValue(items[i], "Shop" + (j + 1) + "Level:"));
                }

            }

        }
    }
   

    private static string hash="9452@abc";

    public static string Encrypt(string input)
    {
        byte[] data = UTF8Encoding.UTF8.GetBytes(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB,Padding= PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateEncryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(results, 0, results.Length);
            }
        }
    }

    public static string Decrypt(string input)
    {
        byte[] data = Convert.FromBase64String(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB,Padding= PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateDecryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return UTF8Encoding.UTF8.GetString(results);
            }
        }
    }
    public static string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("?"))
            value = value.Remove(value.IndexOf("?"));
        return value;
    }
   
}


[Serializable]
class Notebook_Data
{
    public  string AccountID;
    public  string PlayerName;
    public int Money;
    public string LandData;
    
}