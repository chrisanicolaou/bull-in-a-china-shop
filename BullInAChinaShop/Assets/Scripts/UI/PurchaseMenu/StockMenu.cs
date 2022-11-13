using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using CharaGaming.BullInAChinaShop.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.PurchaseMenu
{
    public class StockMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gridNode;
        
        [SerializeField]
        private GameObject _lockedGridNode;
        
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
        private TextMeshProUGUI _quantityText;

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

        private Dictionary<BaseStock, GameObject> _loadedStock = new Dictionary<BaseStock, GameObject>();

        private void Start()
        {
            if (!_isStockLoaded) LoadStock();
        }

        private void OnDisable()
        {
            UnloadStock();
        }

        private void LoadStock()
        {
            GameManager.Instance.AvailableStock.ForEach((stock) =>
            {
                var gridNodeObj = Instantiate(_gridNode, _stockContentArea, false);
                gridNodeObj.FindComponentInChildWithTag<Image>("StockImage").sprite = Resources.Load<Sprite>(stock.SpriteFilePath);
                var btn = gridNodeObj.AddComponent<Button>();
                btn.onClick.AddListener(() => LoadStockPreview(stock));
                _loadedStock[stock] = gridNodeObj;
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
                _stockUpgradeButton.interactable = stock.UpgradeCost <= GameManager.Instance.Cash;

                _stockUpgradeButtonText.text = $"Upgrade $ <color=\"red\">{stock.UpgradeCost}</color>";
                _stockUpgradeButton.onClick.RemoveAllListeners();
                _stockUpgradeButton.onClick.AddListener(() => UpgradeStock(stock));
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
                _quantityText.text = $"<color=\"green\">{intVal.KiloFormat()}";
                _stockPurchaseButtonText.text = $"Buy\n<color=\"red\">$ {(intVal * stock.PurchaseCost).KiloFormat()}";
            });
            
            _quantityText.text = "<color=\"green\">1";
            _stockPurchaseButtonText.text = $"Buy\n<color=\"red\">$ {(1 * stock.PurchaseCost).KiloFormat()}";
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
            GameManager.Instance.Cash -= stock.UpgradeCost;
            stock.Upgrade();

            var stockImg = _loadedStock[stock].FindComponentInChildWithTag<Image>("StockImage");
            stockImg.sprite = Resources.Load<Sprite>(stock.SpriteFilePath);

            LoadStockPreview(stock);
        }

        private void UnloadStock()
        {
            _selectedStockArea.SetActive(false);
        }
    }
}