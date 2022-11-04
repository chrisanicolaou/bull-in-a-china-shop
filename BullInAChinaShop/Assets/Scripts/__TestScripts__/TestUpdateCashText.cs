using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using TMPro;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.__TestScripts__
{
    public class TestUpdateCashText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _cashText;

        private void Start()
        {
            _cashText.text = GameManager.Instance.Cash.ToString();
            GameEventsManager.Instance.AddListener(GameEvent.ItemPurchased, UpdateCashText);
            GameEventsManager.Instance.AddListener(GameEvent.ItemSold, UpdateCashText);
        }

        private void UpdateCashText(Dictionary<string, object> obj)
        {
            _cashText.text = GameManager.Instance.Cash.ToString();
        }
    }
}
