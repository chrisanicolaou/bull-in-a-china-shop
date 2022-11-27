using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using CharaGaming.BullInAChinaShop.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _cashText;

        [SerializeField]
        private Transform _stockUIContentTransform;
        
        [SerializeField] 
        private GameObject _stockUIPrefab;
        
        [SerializeField]
        private TextMeshProUGUI _daysRemainingText;

        private readonly Dictionary<BaseStock, Dictionary<Image, TextMeshProUGUI>> _stockLookup = new();

        private void Start()
        {
            _cashText.text = GameManager.Instance.Cash.KiloFormat();
            _daysRemainingText.text = (GameManager.Instance.TotalNumOfDays - GameManager.Instance.DayNum).ToString().ToTMProColor(Color.red);
            
            GameManager.Instance.AvailableStock.ForEach(s =>
            {
                var obj = Instantiate(_stockUIPrefab, _stockUIContentTransform, false);
                var img = obj.GetComponentInChildren<Image>();
                img.sprite = Resources.Load<Sprite>(s.SpriteFilePath);
                var text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = s.AvailableQuantity.KiloFormat().ToTMProColor(s.AvailableQuantity > 0 ? Color.green : Color.red);
                _stockLookup[s] = new Dictionary<Image, TextMeshProUGUI>
                {
                    [img] = text
                };
            });
            GameEventsManager.Instance.AddListener(GameEvent.StockUpgraded, OnStockUpgrade);
            GameEventsManager.Instance.AddListener(GameEvent.StockDestroyed, OnStockDestroyed);
            GameEventsManager.Instance.AddListener(GameEvent.ItemPurchased, OnStockPurchaseOrSold);
            GameEventsManager.Instance.AddListener(GameEvent.ItemSold, OnStockPurchaseOrSold);
            GameEventsManager.Instance.AddListener(GameEvent.CashChanged, OnCashChange);
        }

        private void OnStockDestroyed(Dictionary<string, object> message)
        {
            var stock = GetStockFromMessage(message);
            foreach (var kvp in _stockLookup[stock])
            {
                kvp.Value.text = stock.AvailableQuantity.KiloFormat().ToTMProColor(stock.AvailableQuantity > 0 ? Color.green : Color.red);
            }
        }

        private void OnCashChange(Dictionary<string, object> obj)
        {
            _cashText.text = GameManager.Instance.Cash.KiloFormat();
        }

        private void OnStockPurchaseOrSold(Dictionary<string, object> message)
        {
            var stock = GetStockFromMessage(message);
            foreach (var kvp in _stockLookup[stock])
            {
                kvp.Value.text = stock.AvailableQuantity.KiloFormat().ToTMProColor(stock.AvailableQuantity > 0 ? Color.green : Color.red);
            }

            _cashText.text = GameManager.Instance.Cash.KiloFormat();
        }

        private void OnStockUpgrade(Dictionary<string, object> message)
        {
            var stock = GetStockFromMessage(message);
            foreach (var kvp in _stockLookup[stock])
            {
                kvp.Key.sprite = Resources.Load<Sprite>(stock.SpriteFilePath);
            }

            _cashText.text = GameManager.Instance.Cash.KiloFormat();
        }

        private BaseStock GetStockFromMessage(Dictionary<string, object> message)
        {
            return (BaseStock)message["item"];
        }

        private void OnDestroy()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.StockUpgraded, OnStockUpgrade);
            GameEventsManager.Instance.RemoveListener(GameEvent.StockDestroyed, OnStockDestroyed);
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemPurchased, OnStockPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemSold, OnStockPurchaseOrSold);
            GameEventsManager.Instance.RemoveListener(GameEvent.CashChanged, OnCashChange);
        }
    }
}