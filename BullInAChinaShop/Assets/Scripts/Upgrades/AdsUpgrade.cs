using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class AdsUpgrade : BaseUpgrade
    {
        private float[] _spawnTimeDecrease = { 0.15f, 0.3f };
        public override int[] UpgradeCosts { get; set; } = { 300, 500 };
        public override string[] Names { get; set; } = { "Newspaper Ads", "BikBok Ads" };
        public override string[] Descriptions { get; set; } = { "Customers appear <color=\"green\">{0}%</color> more often." };

        public override string Description => string.Format(Descriptions[0], _spawnTimeDecrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            Debug.Log(SpriteFilePath);
            GameManager.Instance.SpawnTimeMultiplier = 1f - _spawnTimeDecrease[UpgradeLevel - 1];
        }
    }
}