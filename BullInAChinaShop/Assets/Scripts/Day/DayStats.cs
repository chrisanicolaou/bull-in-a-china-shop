using System.Collections.Generic;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class DayStats
    {
        public int CashEarned { get; set; }
        
        public int ShoppersServed { get; set; }

        public List<ShopperReview> Reviews { get; set; } = new List<ShopperReview>();
    }
}