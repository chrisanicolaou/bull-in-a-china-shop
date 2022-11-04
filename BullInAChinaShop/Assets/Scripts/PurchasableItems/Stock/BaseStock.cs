using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using TMPro;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public abstract class BaseStock : PurchasableItem
    {
        public abstract StockType Type { get; set; }

        private TextMeshProUGUI _costText;

        public override void Start()
        {
            _costText = GetComponentInChildren<TextMeshProUGUI>();
            base.Start();
        }

        public override void UpdatePurchasable(Dictionary<string, object> dictionary)
        {
            _costText.text = Cost > GameManager.Instance.Cash ? $"<color=\"red\">{Cost}</color>" : $"<color=\"green\">{Cost}</color>";
        }

        public override void OnPurchase()
        {
            if (GameManager.Instance.AvailableStock.ContainsKey(Type))
            {
                GameManager.Instance.AvailableStock[Type]++;
            }
            else
            {
                GameManager.Instance.AvailableStock[Type] = 1;
            }
        }
    }
}
