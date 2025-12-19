using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mkey
{
    public class ShopThingHelper : MonoBehaviour
    {
        public Image thingImage;
        public Image thingLabelImage;
        public Text thingTextCount;
        public Text thingTextCountOld;
        public Text thingTextPrice;
        public Button thingBuyButton;

        public void SetData(ShopThingData shopThingData)
        {
            thingImage.sprite = shopThingData.thingImage;
            thingImage.SetNativeSize();  
            thingTextCount.text = (shopThingData.thingCount>0) ?  shopThingData.thingCount.ToString() : "";
            thingTextCountOld.text = (shopThingData.thingCountOld>0) ? shopThingData.thingCountOld.ToString() : "";
            thingTextPrice.text =shopThingData.currency + shopThingData.thingPrice.ToString();

            if (shopThingData.thingLabelImage)
            {
                thingLabelImage.sprite = shopThingData.thingLabelImage;
                thingLabelImage.SetNativeSize();
            }
            else
            {
                thingLabelImage.gameObject.SetActive(false);
            }
        }

        public static ShopThingHelper CreateThing(ShopThingHelper prefab, RectTransform parent, ShopThingData shopThingData)
        {
            prefab.SetData(shopThingData);
            ShopThingHelper shopThing = Instantiate(prefab);
            shopThing.transform.localScale = parent.transform.lossyScale;
            shopThing.transform.SetParent(parent.transform);
            shopThing.thingBuyButton.onClick.RemoveAllListeners();
            shopThing.thingBuyButton.onClick = shopThingData.clickEvent;
            return shopThing;
        }
    }
}