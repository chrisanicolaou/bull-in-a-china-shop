using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class TillUpgrade : BaseUpgrade
    {
        private float[] _thinkTimeIncrease = { 0.1f, 0.25f, };
        public override int[] UpgradeCosts { get; set; } = { 200, 400, };
        public override string[] Names { get; set; } = { "Industrial Till", "Touch Till" };
        public override string[] Descriptions { get; set; } = { "Customers think <color=\"green\">{0}%</color> faster." };

        public override string Description => string.Format(Descriptions[0], _thinkTimeIncrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);

        public override void UpgradeEffect()
        {
            GameObject.FindWithTag("Till").GetComponent<Till>().Upgrade();
            GameManager.Instance.ShopperThinkTimeMultiplier = 1.0f - _thinkTimeIncrease[UpgradeLevel - 1];
        }
    }
}