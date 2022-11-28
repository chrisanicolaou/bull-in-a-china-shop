using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public class BullDestructionUpgrade : BaseUpgrade
    {
        private float[] _destructionModifier = { 0.15f, 0.25f, 0.4f, 0.7f };
        public override int[] UpgradeCosts { get; set; } = { 120, 350, 700, 1200 };
        public override string[] Names { get; set; } = { "Online Reputation" };
        public override string[] Descriptions { get; set; } = { "Become more respectable. Mr. Bull destroys <color=#357D2D>{0}%</color> less each visit." };

        public override string Description => string.Format(Descriptions[0], _destructionModifier[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)] * 100f);
        public override void UpgradeEffect()
        {
            GameManager.Instance.BullDestructionMultiplier = 1.0f - _destructionModifier[UpgradeLevel - 1];
        }
    }
}