using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class Jug : BaseStock
    {
        public override StockType Type { get; set; } = StockType.Jug;
        public override int PurchaseCost { get; set; } = 20;
        public override string[] Names { get; set; } = new[] { "Glass Jug", "Milk Jug", "Porcelain Jug", "Gold Jug" };
        public override string[] FlavourTexts { get; set; } = new[] { "For both baking AND serving!", "Milk sold separately", "What a nice jug", "Fit for a king", };
        public override int[] SellValues { get; set; } = new[] { 35, 60, 110, 250 };
        public override int[] UpgradeCosts { get; set; } = new[] { 75, 180, 350 };

        public override int UnlockCost { get; protected set; } = 100;
    }
}