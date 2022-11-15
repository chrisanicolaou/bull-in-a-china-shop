using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class FruitBowl : BaseStock
    {
        public override StockType Type { get; set; } = StockType.FruitBowl;
        public override int PurchaseCost { get; set; } = 5;
        public override string[] Names { get; set; } = new[] { "Fruit Bowl", "Old Plate 2", "Old Plate 3", "Old Plat 4e", "Old Plate 5", "Old Plate 6", };
        public override string[] FlavourTexts { get; set; } = new[] { "A sweet place to store vegetables", "Old Plate 2", "Old Plate 3", "Old Plat 4e", "Old Plate 5", "Old Plate 6", };
        public override int[] SellValues { get; set; } = new[] { 8, 11, 15, 20, 25, 40 };
        public override int[] UpgradeCosts { get; set; } = new[] { 10, 20, 40, 80, 100 };
    }
}
