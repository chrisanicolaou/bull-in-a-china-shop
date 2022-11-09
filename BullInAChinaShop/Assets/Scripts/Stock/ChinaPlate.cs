using CharaGaming.BullInAChinaShop.Enums;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class ChinaPlate : BaseStock
    {
        public override StockType Type { get; set; } = StockType.ChinaPlate;

        public override string[] Names { get; set; } = new[] { "China Plate", "China Plate Two", "China Plate Three", "China Plate Four", "China Plate Five", "China Plate Five Again" };
        
        public override string[] FlavourTexts { get; set; } = new[] { "This is China Plate", "This is China Plate Two", "This is China Plate Three", "This is China Plate Four", "This is China Plate Five", "This is China Plate ... again" };

        public override int[] SellValues { get; set; } = new[] { 15, 25, 40, 55, 75, 100 };

        public override int[] UpgradeCosts { get; set; } = new[] { 50, 100, 200, 400, 800 };

        public override int PurchaseCost { get; set; } = 10;
    }
}