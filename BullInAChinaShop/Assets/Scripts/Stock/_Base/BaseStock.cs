using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.PurchasableItems;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Tooltip;
using CharaGaming.BullInAChinaShop.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public abstract class BaseStock
    {

        private int _upgradeLevel;
        
        public abstract StockType Type { get; set; }

        public int AvailableQuantity { get; set; } = 0;

        public abstract int PurchaseCost { get; set; }

        public abstract string[] Names { get; set; }
        
        public abstract string[] FlavourTexts { get; set; }
        
        public abstract int[] SellValues { get; set; }
        
        public abstract int[] UpgradeCosts { get; set; }

        public string Name => Names[UpgradeLevel];

        public string FlavourText => FlavourTexts[UpgradeLevel];

        public int SellValue => SellValues[UpgradeLevel];

        public int SellValueUpgradeIncrease => SellValues[UpgradeLevel + 1] - SellValues[UpgradeLevel];

        public int UpgradeCost => UpgradeCosts[UpgradeLevel];

        public string SpriteFilePath => $"Stock/{new string(Name.Where(c => !char.IsWhiteSpace(c)).ToArray())}";

        public virtual int UnlockCost
        {
            get => 0;
            protected set => throw new NotImplementedException();
        }

        public int UpgradeLevel => _upgradeLevel;

        public bool IsUpgradable => _upgradeLevel < 5;

        public bool IsUnlocked => UnlockCost == 0;
        
        public void PurchaseItem()
        {
            if (PurchaseCost > GameManager.Instance.Cash) return;

            GameManager.Instance.Cash -= PurchaseCost;
            
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemPurchased, new Dictionary<string, object>{{ "item", this }});
        }

        public void Upgrade()
        {
            _upgradeLevel++;
        }

        public void Unlock()
        {
            UnlockCost = 0;
        }
    }
}
