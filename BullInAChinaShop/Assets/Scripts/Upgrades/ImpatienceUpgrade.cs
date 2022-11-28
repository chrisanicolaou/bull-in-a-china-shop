using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class ImpatienceUpgrade : BaseUpgrade
    {
        private float[] _impatienceIncrease = { 0.15f, 0.30f, 0.7f, 1.3f };
        public override int[] UpgradeCosts { get; set; } = { 200, 400, 900, 1300 };
        public override string[] Names { get; set; } = { "Radio" };
        public override string[] Descriptions { get; set; } = { "Customers wait <color=#357D2D>{0}%</color> longer." };

        public override string Description => string.Format(Descriptions[0], _impatienceIncrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.ShopperImpatienceTimeMultiplier = 1.0f + _impatienceIncrease[UpgradeLevel - 1];
        }
    }
}