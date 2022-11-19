using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.PurchaseMenu
{
    public class PurchaseMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _openStockMenuButton;
        
        [SerializeField]
        private Button _openUpgradeMenuButton;
        
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private TextMeshProUGUI _cashDisplayText;

        [SerializeField]
        private GameObject _stockMenu;

        [SerializeField]
        private GameObject _upgradeMenu;

        private void Awake()
        {
            _openStockMenuButton.onClick.AddListener(() =>
            {
                _upgradeMenu.SetActive(false);
                _stockMenu.SetActive(true);
            });
            _openUpgradeMenuButton.onClick.AddListener(() =>
            {
                _stockMenu.SetActive(false);
                _upgradeMenu.SetActive(true);
            });
            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            UpdateCash(null);
            GameEventsManager.Instance.AddListener(GameEvent.CashChanged, UpdateCash);
        }

        private void UpdateCash(Dictionary<string, object> message)
        {
            _cashDisplayText.text = $"<color=\"yellow\">{GameManager.Instance.Cash.KiloFormat()}</color>";
        }

        private void OnDisable()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.CashChanged, UpdateCash);
            GameEventsManager.Instance.TriggerEvent(GameEvent.PurchaseMenuClosed, null);
        }
    }
}