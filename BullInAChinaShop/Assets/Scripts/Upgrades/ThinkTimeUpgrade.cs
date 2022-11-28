using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class ThinkTimeUpgrade : BaseUpgrade
    {
        private float[] _thinkTimeDecrease = { 0.2f, 0.4f, 0.8f };
        public override int[] UpgradeCosts { get; set; } = { 300, 600, 2000 };
        public override string[] Names { get; set; } = { "Focus Pheromones" };
        public override string[] Descriptions { get; set; } = { "Customers think <color=#357D2D>{0}%</color> faster." };

        public override string Description => string.Format(Descriptions[0], _thinkTimeDecrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.ShopperThinkTimeMultiplier = 1.0f - _thinkTimeDecrease[UpgradeLevel - 1];
        }
    }
}