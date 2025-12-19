using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mkey
{
    public enum RealShopType { None, Coins, DealCoins };

    [Serializable]
    public class ShopThingData
    {
        public string name;
        public Sprite thingImage;
        public Sprite thingLabelImage;
        public int thingCount;
        public int thingCountOld;
        public float thingPrice;
        public string currency;
        public string kProductID;
        public bool showInShop = true;

        [HideInInspector]
        public Button.ButtonClickedEvent clickEvent;

        public ShopThingData(ShopThingData prod)
        {
            name = prod.name;
            thingImage = prod.thingImage;
            thingLabelImage = prod.thingLabelImage;
            thingCount = prod.thingCount;
            thingCountOld = prod.thingCountOld;
            thingPrice = prod.thingPrice;
            currency = prod.currency;
            kProductID = prod.kProductID;
            showInShop = prod.showInShop;
            clickEvent = prod.clickEvent;
        }
    }

    [Serializable]
    public class ShopThingDataReal : ShopThingData
    {
        public RealShopType shopType = RealShopType.Coins;
        [Space(8, order = 0)]
        [Header("Purchase Event: ", order = 1)]
        public UnityEvent PurchaseEvent;

        public ShopThingDataReal(ShopThingDataReal prod) : base(prod)
        {
            shopType = prod.shopType;
            PurchaseEvent = prod.PurchaseEvent;
        }
    }

    [Serializable]
    public class ShopThingDataRealDeal : ShopThingDataReal
    {
        public ShopThingDataRealDeal(ShopThingDataRealDeal prod) : base(prod)
        {
           
        }
    }
}
