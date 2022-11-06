using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.UI.Tooltip;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public class OldPlate : BaseStock
    {
        // Start is called before the first frame update
        public override int Cost { get; set; } = 10;

        public override StockType Type { get; set; } = StockType.OldPlate;
        public override ToolTipInfo[] ToolTipInfos { get; set; } = new[] { new ToolTipInfo("Grandma used to love this plate...", "Old Plate") };

        public override string SpriteFilePath { get; set; } = "Stock/OldPlate";
    }
}
