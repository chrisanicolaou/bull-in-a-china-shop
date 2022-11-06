using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.UI.Tooltip;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public class ChinaPlate : BaseStock
    {
        public override int Cost { get; set; } = 20;
        public override StockType Type { get; set; } = StockType.ChinaPlate;
        public override ToolTipInfo[] ToolTipInfos { get; set; } = new ToolTipInfo[] { new ToolTipInfo("A little cracked, but still looks nice.", "China Plate") };
        public override string SpriteFilePath { get; set; } = "Stock/ChinaPlate";
    }
}