using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class Jug : BaseStock
    {
        public override StockType Type { get; set; } = StockType.Jug;
        public override int PurchaseCost { get; set; } = 20;
        public override string[] Names { get; set; } = new[] { "Jug", "Jug Two", "Jug Three", "Jug Four", "Jug Five", "Jug Six" };
        public override string[] FlavourTexts { get; set; } = new[] { "Often comes in pairs", "Often comes in pairs", "Often comes in pairs", "Often comes in pairs", "Often comes in pairs", "Often comes in pairs", };
        public override int[] SellValues { get; set; } = new[] { 35, 60, 110, 170, 250, 400 };
        public override int[] UpgradeCosts { get; set; } = new[] { 75, 180, 350, 650, 1000 };
    }
}