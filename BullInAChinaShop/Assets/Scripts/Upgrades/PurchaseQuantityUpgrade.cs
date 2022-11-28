using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class PurchaseQuantityUpgrade : BaseUpgrade
    {
        private int[] _minPurchaseQuantity = { 3, 4, 5 };
        public override int[] UpgradeCosts { get; set; } = { 450, 650, 1150 };
        public override string[] Names { get; set; } = { "Cleaner Stock" };
        public override string[] Descriptions { get; set; } = { "Customers buy at least <color=#357D2D>{0}</color> stock per visit." };

        public override string Description => string.Format(Descriptions[0], _minPurchaseQuantity[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)]);
        public override void UpgradeEffect()
        {
            GameManager.Instance.ShopperPurchaseAmount = _minPurchaseQuantity[UpgradeLevel - 1];
        }
    }
}