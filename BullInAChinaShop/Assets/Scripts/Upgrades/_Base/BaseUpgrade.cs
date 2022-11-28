using System.Linq;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Upgrades
{
    public abstract class BaseUpgrade
    {
        private int _upgradeLevel;
        
        public abstract int[] UpgradeCosts { get; set; }

        public int UpgradeLevel => _upgradeLevel;
        
        public bool IsUpgradable => _upgradeLevel < UpgradeCosts.Length;

        public abstract string[] Names { get; set; }
        
        public abstract string[] Descriptions { get; set; }

        public string Name => Names[Mathf.Min(UpgradeLevel, Names.Length - 1)];

        public virtual string Description => Descriptions[Mathf.Min(UpgradeLevel, Descriptions.Length - 1)];

        public int UpgradeCost => UpgradeCosts[Mathf.Min(UpgradeLevel, UpgradeCosts.Length - 1)];

        public string SpriteFilePath => $"Upgrades/{new string(Name.Where(c => !char.IsWhiteSpace(c)).ToArray())}";

        public void Upgrade()
        {
            _upgradeLevel++;
            UpgradeEffect();
        }

        public abstract void UpgradeEffect();
    }
}