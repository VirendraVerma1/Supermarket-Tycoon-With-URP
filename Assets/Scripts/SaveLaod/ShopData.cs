using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData 
{
    public string ShopName;
    public int ShopQuantity;
    public int ShopDiscount;
    public int ShopEmployee;
    public int ShopLevel;

    public ShopData(string shopName, int shopQuantity, int shopDiscount, int shopEmployee, int shopLevel)
    {
        ShopName=shopName;
        ShopQuantity=shopQuantity;
        ShopDiscount=shopDiscount;
        ShopEmployee=shopEmployee;
        ShopLevel = shopLevel;
    }
}
