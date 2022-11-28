using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class FruitBowl : BaseStock
    {
        public override StockType Type { get; set; } = StockType.FruitBowl;
        public override int PurchaseCost { get; set; } = 5;
        public override string[] Names { get; set; } = { "Wooden Bowl", "Glass Bowl", "Florette Bowl", "Gold Bowl", };
        public override string[] FlavourTexts { get; set; } = { "A sweet place to store vegetables", "Now you can see the inside!", "A pretty place to store vegetables", "They say eating from this bowl leads to eternal life", };
        public override int[] SellValues { get; set; } = { 20, 30, 35, 40, };
        public override int[] UpgradeCosts { get; set; } = { 50, 100, 200, };
    }
}
