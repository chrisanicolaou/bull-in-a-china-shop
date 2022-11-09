using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI
{
    public class UpgradeMenu : MonoBehaviour
    {
        [SerializeField]
        private Transform _stockContentArea;

        [SerializeField]
        private GameObject _selectedStockArea;

        [SerializeField]
        private Image _stockPreviewImage;

        [SerializeField]
        private TextMeshProUGUI _stockCostText;
        
        [SerializeField]
        private TextMeshProUGUI _stockSellValueText;
        
        [SerializeField]
        private TextMeshProUGUI _stockNameText;
        
        [SerializeField]
        private TextMeshProUGUI _stockFlavourText;

        [SerializeField]
        private Slider _stockQuantitySlider;

        [SerializeField]
        private Button _stockPurchaseButton;

        [SerializeField]
        private TextMeshProUGUI _stockPurchaseButtonText;

        [SerializeField]
        private Button _stockUpgradeButton;

        [SerializeField]
        private TextMeshProUGUI _stockUpgradeButtonText;

        private bool _isStockLoaded;

        private bool _areUpgradesLoaded;

        private Dictionary<BaseStock, GameObject> LoadedStock = new Dictionary<BaseStock, GameObject>();

        private void Start()
        {
            if (!_isStockLoaded) LoadStock();
            // _exitButton.onClick.AddListener(() => gameObject.SetActive(false));
            // _stockMenuButton.onClick.AddListener(() =>
            // {
            //     _upgradeMenuObj.SetActive(false);
            //     _stockMenuObj.SetActive(true);
            // });
            // _upgradeMenuButton.onClick.AddListener(() =>
            // {
            //     _stockMenuObj.SetActive(false);
            //     _upgradeMenuObj.SetActive(true);
            //     if (_areUpgradesLoaded) LoadUpgrades();
            // });
        }

        private void LoadStock()
        {
            GameManager.Instance.AvailableStock.ForEach((stock) =>
            {
                var stockObj = new StockBuilder()
                    .SetParent(_stockContentArea)
                    .SetStock(stock)
                    .AsPurchasable()
                    .Build();
                var btn = stockObj.AddComponent<Button>();
                btn.onClick.AddListener(() => LoadStockPreview(stock));
                LoadedStock[stock] = stockObj;
            });
        }

        private void LoadStockPreview(BaseStock stock)
        {
            if (!_selectedStockArea.activeSelf) _selectedStockArea.SetActive(true);

            _stockPreviewImage.sprite = Resources.Load<Sprite>(stock.SpriteFilePath);
            _stockCostText.text = stock.PurchaseCost.ToString();
            _stockSellValueText.text = stock.SellValue.ToString();
            _stockNameText.text = stock.Name;
            _stockFlavourText.text = stock.FlavourText;

            if (stock.IsUpgradable)
            {
                if (stock.UpgradeCost > GameManager.Instance.Cash)
                {
                    _stockUpgradeButtonText.text = "Not enough moola";
                    _stockUpgradeButton.interactable = false;
                }
                else
                {
                    _stockUpgradeButtonText.text = $"Upgrade <color=\"red\">-{stock.UpgradeCost}</color>";
                    _stockUpgradeButton.onClick.RemoveAllListeners();
                    _stockUpgradeButton.onClick.AddListener(() => UpgradeStock(stock));
                }
            }
            else
            {
                _stockUpgradeButtonText.text = "Max level reached";
                _stockUpgradeButton.interactable = false;
            }

            bool canPurchase = stock.PurchaseCost <= GameManager.Instance.Cash;

            if (!canPurchase)
            {
                _stockQuantitySlider.value = 0;
                _stockQuantitySlider.minValue = 0;
                _stockQuantitySlider.maxValue = 0;
                _stockPurchaseButtonText.text = "Not enough moola";
                _stockPurchaseButton.interactable = false;
                return;
            }
            
            _stockQuantitySlider.value = 1;
            _stockQuantitySlider.minValue = 1;
            _stockQuantitySlider.maxValue = Mathf.FloorToInt((float)(GameManager.Instance.Cash) / stock.PurchaseCost);
            _stockQuantitySlider.onValueChanged.RemoveAllListeners();
            _stockQuantitySlider.onValueChanged.AddListener((value) =>
            {
                var intVal = (int)value;
                _stockPurchaseButtonText.text = $"Buy <color=\"red\">{value}";
            });
            
            _stockPurchaseButtonText.text = $"Buy <color=\"red\">1";
            _stockPurchaseButton.onClick.RemoveAllListeners();
            _stockPurchaseButton.onClick.AddListener(() =>
            {
                for (int i = 0; i < _stockQuantitySlider.value; i++)
                {
                    stock.PurchaseItem();
                }

                LoadStockPreview(stock);
            });
        }

        private void UpgradeStock(BaseStock stock)
        {
            stock.Upgrade();
            
            var stockImg = LoadedStock[stock].FindComponentInChildWithTag<Image>("StockImage");
            stockImg.sprite = Resources.Load<Sprite>(stock.SpriteFilePath);

            LoadStockPreview(stock);
        }
    }
}
