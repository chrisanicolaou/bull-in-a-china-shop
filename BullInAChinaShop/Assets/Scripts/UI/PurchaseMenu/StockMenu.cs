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
        private TextMeshProUGUI _stockUpgradedSellValueText;

        private Hoverable _stockUpgradeHover;

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

        private readonly Dictionary<BaseStock, GameObject> _loadedStock = new Dictionary<BaseStock, GameObject>();

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
                var gridNodeObj = Instantiate(stock.IsUnlocked ? _gridNode : _lockedGridNode, _stockContentArea, false);
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

            if (stock.IsUpgradable && stock.IsUnlocked)
            {
                _stockUpgradeButton.interactable = stock.UpgradeCost <= GameManager.Instance.Cash;

                _stockUpgradeButtonText.text = $"Upgrade\n<color=\"red\">$ {stock.UpgradeCost}</color>";
                _stockUpgradeButton.onClick.RemoveAllListeners();
                _stockUpgradeButton.onClick.AddListener(() => UpgradeStock(stock));
                _stockUpgradeHover ??= _stockUpgradeButton.GetComponent<Hoverable>();
                _stockUpgradedSellValueText.text = $"<color=\"green\">+{stock.SellValueUpgradeIncrease}</color>";
                _stockUpgradeHover.OnHover(() => _stockUpgradedSellValueText.gameObject.SetActive(true));
                _stockUpgradeHover.OnExit(() => _stockUpgradedSellValueText.gameObject.SetActive(false));
            }
            else if (!stock.IsUnlocked)
            {
                _stockUpgradeButton.interactable = true;

                _stockUpgradeButtonText.text = $"Unlock\n<color=\"red\">$ {stock.UnlockCost}</color>";
                _stockUpgradeButton.onClick.RemoveAllListeners();
                _stockUpgradeButton.onClick.AddListener(() => UnlockStock(stock));
                if (_stockUpgradeHover != null) _stockUpgradeHover.Clear();
                
                _stockQuantitySlider.value = 0;
                _stockQuantitySlider.minValue = 0;
                _stockQuantitySlider.maxValue = 0;
                _stockPurchaseButton.interactable = false;
                return;
            }
            else 
            {
                _stockUpgradeButtonText.text = "Max level reached";
                _stockUpgradeButton.interactable = false;
                _stockUpgradeHover.Clear();
            }

            var canPurchase = stock.PurchaseCost <= GameManager.Instance.Cash;

            if (!canPurchase)
            {
                _stockQuantitySlider.value = 0;
                _stockQuantitySlider.minValue = 0;
                _stockQuantitySlider.maxValue = 0;
                _stockPurchaseButton.interactable = false;
                return;
            }
            
            _stockPurchaseButton.interactable = true;
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

        private void UnlockStock(BaseStock stock)
        {
            stock.Unlock();
            var stockObj = _loadedStock[stock];
            var siblingIndex = stockObj.GetComponent<RectTransform>().GetSiblingIndex();
            Destroy(stockObj);
            
            var gridNodeObj = Instantiate(_gridNode, _stockContentArea, false);
            gridNodeObj.FindComponentInChildWithTag<Image>("StockImage").sprite = Resources.Load<Sprite>(stock.SpriteFilePath);
            var btn = gridNodeObj.AddComponent<Button>();
            btn.onClick.AddListener(() => LoadStockPreview(stock));
            gridNodeObj.GetComponent<RectTransform>().SetSiblingIndex(siblingIndex);
            _loadedStock[stock] = gridNodeObj;
            
            LoadStockPreview(stock);
        }

        private void UnloadStock()
        {
            _selectedStockArea.SetActive(false);
        }
    }
}