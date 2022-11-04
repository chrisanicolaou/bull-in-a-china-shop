using CharaGaming.BullInAChinaShop.Enums;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public class BasicPlate : BaseStock
    {
        // Start is called before the first frame update
        public override int Cost { get; set; } = 10;

        public override StockType Type { get; set; } = StockType.BasicPlate;
    }
}
