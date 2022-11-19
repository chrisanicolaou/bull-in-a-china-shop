using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class ImpatienceUpgrade : BaseUpgrade
    {
        private float[] _impatienceIncrease = { 0.1f, 0.2f };
        public override int[] UpgradeCosts { get; set; } = { 200, 400 };
        public override string[] Names { get; set; } = { "Radio", "Wifi" };
        public override string[] Descriptions { get; set; } = { "Customers wait <color=\"green\">{0}%</color> longer." };

        public override string Description => string.Format(Descriptions[0], _impatienceIncrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.ShopperImpatienceTimeMultiplier = 1.0f + _impatienceIncrease[UpgradeLevel - 1];
        }
    }
}