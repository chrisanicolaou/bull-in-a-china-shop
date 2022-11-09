using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.PurchasableItems
{
    public interface IPurchasableItem
    {
        public int PurchaseCost { get; set; }

        public void PurchaseItem();

        public void UpdatePurchasable();
    }
}
