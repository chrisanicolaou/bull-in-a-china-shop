using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.UI.Tooltip;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public class Jug : BaseStock
    {
        public override int Cost { get; set; } = 25;
        public override StockType Type { get; set; } = StockType.Jug;
        public override ToolTipInfo[] ToolTipInfos { get; set; } = new ToolTipInfo[] { new ToolTipInfo("Often comes in pairs", "Jug") };
        public override string SpriteFilePath { get; set; } = "Stock/Jug";
    }
}