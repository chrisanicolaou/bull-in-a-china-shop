using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using CharaGaming.BullInAChinaShop.Upgrades;
using CharaGaming.BullInAChinaShop.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.PurchaseMenu
{
    public class UpgradeMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gridNode;
        
        [SerializeField]
        private Transform _upgradeContentArea;

        [SerializeField]
        private GameObject _selectedUpgradeArea;

        [SerializeField]
        private Image _upgradePreviewImage;

        [SerializeField]
        private Transform _upgradeBeanHolder;

        [SerializeField]
        private GameObject _upgradeBeanFilled;

        [SerializeField]
        private GameObject _upgradeBeanEmpty;

        [SerializeField]
        private TextMeshProUGUI _upgradeNameText;

        [SerializeField]
        private TextMeshProUGUI _upgradeDescription;

        [SerializeField]
        private Button _upgradeButton;

        [SerializeField]
        private TextMeshProUGUI _upgradeButtonText;

        private bool _isLoaded;

        private readonly Dictionary<BaseUpgrade, GameObject> _loadedUpgrades = new Dictionary<BaseUpgrade, GameObject>();

        private void Start()
        {
            if (!_isLoaded) LoadUpgrades();
        }

        private void OnDisable()
        {
            UnloadUpgrades();
        }

        private void LoadUpgrades()
        {
            GameManager.Instance.Upgrades.ForEach((upgrade) =>
            {
                var gridNodeObj = Instantiate(_gridNode, _upgradeContentArea, false);
                gridNodeObj.FindComponentInChildWithTag<Image>("StockImage").sprite = Resources.Load<Sprite>(upgrade.SpriteFilePath);
                var btn = gridNodeObj.AddComponent<Button>();
                btn.onClick.AddListener(() => LoadUpgradePreview(upgrade));
                _loadedUpgrades[upgrade] = gridNodeObj;
            });
        }

        private void LoadUpgradePreview(BaseUpgrade upgrade)
        {
            if (!_selectedUpgradeArea.activeSelf) _selectedUpgradeArea.SetActive(true);

            _upgradePreviewImage.sprite = Resources.Load<Sprite>(upgrade.SpriteFilePath);
            _upgradeNameText.text = upgrade.Name;
            _upgradeDescription.text = upgrade.Description;

            if (upgrade.IsUpgradable)
            {
                _upgradeButton.interactable = upgrade.UpgradeCost <= GameManager.Instance.Cash;

                _upgradeButtonText.text = $"Upgrade\n{$"$ {upgrade.UpgradeCost}".ToTMProColor(Color.red)}";
                _upgradeButton.onClick.RemoveAllListeners();
                _upgradeButton.onClick.AddListener(() => UpgradeSelected(upgrade));
            }
            else 
            {
                _upgradeButtonText.text = "Max level reached";
                _upgradeButton.interactable = false;
            }
            
            _upgradeBeanHolder.DestroyAllChildren();

            for (var i = 0; i <= upgrade.UpgradeCosts.Length; i++)
            {
                Instantiate(upgrade.UpgradeLevel >= i ? _upgradeBeanFilled : _upgradeBeanEmpty, _upgradeBeanHolder);
            }
        }

        private void UpgradeSelected(BaseUpgrade upgrade)
        {
            GameManager.Instance.Cash -= upgrade.UpgradeCost;
            upgrade.Upgrade();

            var stockImg = _loadedUpgrades[upgrade].FindComponentInChildWithTag<Image>("StockImage");
            stockImg.sprite = Resources.Load<Sprite>(upgrade.SpriteFilePath);

            LoadUpgradePreview(upgrade);
        }

        private void UnloadUpgrades()
        {
            _selectedUpgradeArea.SetActive(false);
        }
    }
}