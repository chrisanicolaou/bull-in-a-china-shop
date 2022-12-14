using System.Collections.Generic;
using System.Linq;
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
                
                var upgradeSprite = Resources.Load<Sprite>(upgrade.SpriteFilePath) ??
                                    SpriteHelpers.LoadFromSheet("Upgrades/UpgradesSheet", new string(upgrade.Name.Where(c => !char.IsWhiteSpace(c)).ToArray()));
                
                var img = gridNodeObj.FindComponentInChildWithTag<Image>("StockImage");
                img.sprite = upgradeSprite;
                img.rectTransform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                img.SetNativeSize();
                var btn = gridNodeObj.AddComponent<Button>();
                btn.onClick.AddListener(() => LoadUpgradePreview(upgrade));
                gridNodeObj.FindComponentInChildWithTag<Image>("QuantityNode").gameObject.SetActive(false);
                _loadedUpgrades[upgrade] = gridNodeObj;
            });
        }

        private void LoadUpgradePreview(BaseUpgrade upgrade)
        {
            if (!_selectedUpgradeArea.activeSelf) _selectedUpgradeArea.SetActive(true);
                
            var upgradeSprite = Resources.Load<Sprite>(upgrade.SpriteFilePath) ??
                                SpriteHelpers.LoadFromSheet("Upgrades/UpgradesSheet", new string(upgrade.Name.Where(c => !char.IsWhiteSpace(c)).ToArray()));

            _upgradePreviewImage.sprite = upgradeSprite;
            _upgradePreviewImage.SetNativeSize();
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

            var upgradeSprite = Resources.Load<Sprite>(upgrade.SpriteFilePath) ??
                                SpriteHelpers.LoadFromSheet("Upgrades/UpgradesSheet", new string(upgrade.Name.Where(c => !char.IsWhiteSpace(c)).ToArray()));
            var img = _loadedUpgrades[upgrade].FindComponentInChildWithTag<Image>("StockImage");
            img.sprite = upgradeSprite;
            img.rectTransform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            img.SetNativeSize();

            LoadUpgradePreview(upgrade);
        }

        private void UnloadUpgrades()
        {
            _selectedUpgradeArea.SetActive(false);
        }
    }
}