using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class Plate : BaseStock
    {
        public override StockType Type { get; set; } = StockType.Plate;

        public override string[] Names { get; set; } = new[] { "Chipped Plate", "Old Plate", "China Plate", "Gold Plate" };
        
        public override string[] FlavourTexts { get; set; } = new[] { "Bits of ceramic don't taste too bad", "At least its not chipped", "What a lovely plate", "Fit for a king" };

        public override int[] SellValues { get; set; } = new[] { 15, 25, 55, 100 };

        public override int[] UpgradeCosts { get; set; } = new[] { 50, 100, 200 };

        public override int PurchaseCost { get; set; } = 10;
    }
}