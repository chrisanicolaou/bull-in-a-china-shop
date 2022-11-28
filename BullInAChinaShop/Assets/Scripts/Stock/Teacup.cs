using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class Teacup : BaseStock
    {
        public override StockType Type { get; set; } = StockType.Teacup;
        public override int PurchaseCost { get; set; } = 10;
        public override string[] Names { get; set; } = { "Rusty Teacup", "Noisy Teacup", "Florette Teacup", "Gold Teacup", };
        public override string[] FlavourTexts { get; set; } = { "Drink at your own risk", "Not to everyone's taste", "Nice to look at - hard to drink from!", "It's golden", };
        public override int[] SellValues { get; set; } = { 50, 75, 95, 120, };
        public override int[] UpgradeCosts { get; set; } = { 125, 200, 250, };
    }
}