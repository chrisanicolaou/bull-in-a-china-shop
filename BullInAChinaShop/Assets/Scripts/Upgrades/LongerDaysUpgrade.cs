using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class LongerDaysUpgrade : BaseUpgrade
    {
        private float[] _dayDurationIncrease = { 0.1f, 0.3f, 0.5f };
        public override int[] UpgradeCosts { get; set; } = { 200, 600, 1500 };
        public override string[] Names { get; set; } = { "Magic Clock" };
        public override string[] Descriptions { get; set; } = { "Days last <color=#357D2D>{0}%</color> longer." };

        public override string Description => string.Format(Descriptions[0], _dayDurationIncrease[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.DayDuration = Mathf.RoundToInt(GameManager.Instance.DayDuration * (1.0f + _dayDurationIncrease[UpgradeLevel - 1]));
        }
    }
}