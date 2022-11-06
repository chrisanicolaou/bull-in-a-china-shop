using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using TMPro;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.__TestScripts__
{
    public class TestUpdateStockText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _stockText;

        private void Start()
        {
            _stockText.text = GameManager.Instance.AvailableStock[StockType.OldPlate].ToString();
            GameEventsManager.Instance.AddListener(GameEvent.ItemPurchased, UpdateStockText);
            GameEventsManager.Instance.AddListener(GameEvent.ItemSold, UpdateStockText);
        }

        private void UpdateStockText(Dictionary<string, object> obj)
        {
            _stockText.text = GameManager.Instance.AvailableStock[StockType.OldPlate].ToString();
        }
    }
}
