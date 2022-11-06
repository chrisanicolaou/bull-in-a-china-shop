using System.Collections.Generic;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public static class ShopperUtils
    {
        public static Shopper GetShopperFromMessage(Dictionary<string, object> message)
        {
            if (message.TryGetValue("shopper", out var shopper))
            {
                return (Shopper)shopper;
            }
            
            Debug.LogError("Shopper has not been included in message!");
            return null;
        }
    }
}