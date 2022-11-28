using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class AdsUpgrade : BaseUpgrade
    {
        private float[] _spawnTimeDecrease = { 0.15f, 0.3f, 0.6f };
        public override int[] UpgradeCosts { get; set; } = { 300, 500, 1000 };
        public override string[] Names { get; set; } = { "Newspaper Ads" };
        public override string[] Descriptions { get; set; } = { "Customers appear <color=#357D2D>{0}%</color> more often." };

        public override string Description => string.Format(Descriptions[0], _spawnTimeDecrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.SpawnTimeMultiplier /= 1.0f + _spawnTimeDecrease[UpgradeLevel - 1];
        }
    }
}