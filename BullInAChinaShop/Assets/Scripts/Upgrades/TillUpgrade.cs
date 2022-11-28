using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class TillUpgrade : BaseUpgrade
    {
        private int _timesUpgraded;
        private bool _isSubscribed;
        
        private float[] _sellValueIncreases = { 0.2f, 0.5f, };
        public override int[] UpgradeCosts { get; set; } = { 750, 1500, };
        public override string[] Names { get; set; } = { "Industrial Till", "Touch Till" };
        public override string[] Descriptions { get; set; } = { "ALL stock sells for <color=#357D2D>{0}%</color> more." };

        public override string Description => string.Format(Descriptions[0], _sellValueIncreases[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);

        public override void UpgradeEffect()
        {
            if (!_isSubscribed)
            {
                GameEventsManager.Instance.AddListener(GameEvent.PurchaseMenuClosed, UpgradeTill);
                _isSubscribed = true;
            }
            _timesUpgraded++;
        }

        private void UpgradeTill(Dictionary<string, object> message)
        {
            GameObject.FindWithTag("Till").GetComponent<Till>().Upgrade(_timesUpgraded);
            GameManager.Instance.GlobalSellValueMultiplier *= 1.0f + _sellValueIncreases[UpgradeLevel - 1];
            _timesUpgraded = 0;
            _isSubscribed = false;
            GameEventsManager.Instance.RemoveListener(GameEvent.PurchaseMenuClosed, UpgradeTill);
        }
    }
}