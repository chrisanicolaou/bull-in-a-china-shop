using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class ShelfStacker : MonoBehaviour
    {
        [SerializeField]
        private Transform _plateShelf;

        [SerializeField]
        [Range(0, 60)]
        private int _plateShelfCap = 30;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _plateScale = 0.2f;

        private StockBuilder _stockBuilder;
        
        private void Start()
        {
            _stockBuilder = new StockBuilder().CleanOnBuild();
            
            GameEventsManager.Instance.AddListener(GameEvent.ItemPurchased, OnItemPurchaseOrSold);
            GameEventsManager.Instance.AddListener(GameEvent.ItemSold, OnItemPurchaseOrSold);
            GameEventsManager.Instance.AddListener(GameEvent.StockUpgraded, OnItemUpgraded);
        }

        private void OnItemPurchaseOrSold(Dictionary<string, object> message)
        {
            if (!message.TryGetValue("item", out object item))
            {
                Debug.LogError("Item key not added to event!");
                return;
            }
            var stock = (BaseStock)item;
            var stockType = stock.Type;

            switch (stockType)
            {
                case StockType.Plate:
                    PopulatePlateShelf(stock);
                    break;
            }
        }

        private void OnItemUpgraded(Dictionary<string, object> message)
        {
            if (!message.TryGetValue("item", out object item))
            {
                Debug.LogError("Item key not added to event!");
                return;
            }
            var stock = (BaseStock)item;
            var stockType = stock.Type;

            switch (stockType)
            {
                case StockType.Plate:
                    _plateShelf.DestroyAllChildren();
                    PopulatePlateShelf(stock, true);
                    break;
            }
        }

        private void PopulatePlateShelf(BaseStock stock, bool forceReload = false)
        {
            var plateShelfNum = !forceReload ? _plateShelf.childCount : 0;
            if (plateShelfNum == stock.AvailableQuantity || (plateShelfNum == _plateShelfCap && stock.AvailableQuantity >= _plateShelfCap))
            {
                return;
            }

            var plateDiff = stock.AvailableQuantity - plateShelfNum;
            for (var i = 0; i < Mathf.Min(Mathf.Abs(plateDiff), _plateShelfCap); i++)
            {
                if (plateDiff < 0)
                {
                    Destroy(_plateShelf.GetChild(0).gameObject);
                }
                else
                {
                    _stockBuilder.SetParent(_plateShelf)
                        .SetScale(_plateScale)
                        .SetStock(stock)
                        .SetRotation(new Vector3(-20f, 55f, 0f))
                        .Build();
                }
            }
        }

        private void OnDestroy()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemPurchased, OnItemPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemSold, OnItemPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.StockUpgraded, OnItemUpgraded);
        }
    }
}
