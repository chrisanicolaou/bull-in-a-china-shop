using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.PurchasableItems
{
    public abstract class PurchasableItem : MonoBehaviour
    {
        private bool _isPurchasable;
        
        public abstract int Cost { get; set; }

        public virtual void Start()
        {
            var btn = gameObject.AddButton();
            btn.onClick.AddListener(PurchaseItem);
            GameEventsManager.Instance.AddListener(GameEvent.ItemPurchased, UpdatePurchasable);
            UpdatePurchasable(null);
        }

        protected void PurchaseItem()
        {
            if (Cost > GameManager.Instance.Cash) return;

            GameManager.Instance.Cash -= Cost;
            
            OnPurchase();
            
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemPurchased, null);
        }

        public abstract void UpdatePurchasable(Dictionary<string, object> dictionary);
        public abstract void OnPurchase();
    }
}
