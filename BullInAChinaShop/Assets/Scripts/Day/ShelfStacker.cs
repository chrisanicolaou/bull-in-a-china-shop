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
        private Transform _shelfContainerTransform;
        
        [SerializeField]
        private Vector3[] _backShelfLocations;
        
        [SerializeField]
        private GameObject _plateShelfPrefab;

        private readonly List<RectTransform> _plateShelfTransforms  = new();

        [SerializeField]
        [Range(0, 60)]
        private int _plateShelfCap = 40;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _plateScale = 0.25f;

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
                    PopulatePlateShelves(stock);
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
                    _plateShelfTransforms.ForEach(s => s.DestroyAllChildren());
                    PopulatePlateShelves(stock, true);
                    break;
            }
        }

        private void PopulatePlateShelves(BaseStock stock, bool forceReload = false)
        {
            var normalizedQuantity = stock.AvailableQuantity;
            if (stock.AvailableQuantity > _plateShelfCap) normalizedQuantity -= _plateShelfCap / 2;
            var requiredShelves = Mathf.Min(Mathf.CeilToInt(normalizedQuantity / (float)_plateShelfCap), _backShelfLocations.Length);
            var remainingPlates = Mathf.Min(stock.AvailableQuantity, requiredShelves * _plateShelfCap);

            for (var i = 0; i < requiredShelves; i++)
            {
                RectTransform plateShelfTransform;
                if (i >= _plateShelfTransforms.Count)
                {
                    var plateShelf = Instantiate(_plateShelfPrefab, _shelfContainerTransform, false);
                    plateShelfTransform = plateShelf.GetComponent<RectTransform>();
                    plateShelfTransform.anchoredPosition = _backShelfLocations[i];
                    _plateShelfTransforms.Add(plateShelfTransform);
                }
                else
                {
                    plateShelfTransform = _plateShelfTransforms[i];
                }
                
                var plateShelfNum = !forceReload ? plateShelfTransform.childCount : 0;
                
                if (plateShelfNum == remainingPlates || (plateShelfNum == _plateShelfCap && remainingPlates >= _plateShelfCap))
                {
                    remainingPlates -= plateShelfNum;
                    continue;
                }
                
                var plateDiff = remainingPlates - plateShelfNum;
                for (var j = 0; j < Mathf.Min(Mathf.Abs(plateDiff), _plateShelfCap); j++)
                {
                    if (plateDiff < 0)
                    {
                        Destroy(plateShelfTransform.GetChild(0).gameObject);
                    }
                    else
                    {
                        _stockBuilder.SetParent(plateShelfTransform)
                            .SetScale(_plateScale)
                            .SetStock(stock)
                            .SetRotation(new Vector3(-20f, 55f, 0f))
                            .Build();
                    }
                }
            }
            // var plateShelfNum = !forceReload ? _plateShelf.childCount : 0;
            // if (plateShelfNum == stock.AvailableQuantity || (plateShelfNum == _plateShelfCap && stock.AvailableQuantity >= _plateShelfCap))
            // {
            //     return;
            // }
            //
            // var plateDiff = stock.AvailableQuantity - plateShelfNum;
            // for (var i = 0; i < Mathf.Min(Mathf.Abs(plateDiff), _plateShelfCap); i++)
            // {
            //     if (plateDiff < 0)
            //     {
            //         Destroy(_plateShelf.GetChild(0).gameObject);
            //     }
            //     else
            //     {
            //         _stockBuilder.SetParent(_plateShelf)
            //             .SetScale(_plateScale)
            //             .SetStock(stock)
            //             .SetRotation(new Vector3(-20f, 55f, 0f))
            //             .Build();
            //     }
            // }
        }

        private void OnDestroy()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemPurchased, OnItemPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemSold, OnItemPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.StockUpgraded, OnItemUpgraded);
        }
    }
}
