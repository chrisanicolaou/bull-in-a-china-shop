using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Utils;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public int Cash { get; set; } = 500;

        public Dictionary<StockType, int> AvailableStock { get; set; } = new Dictionary<StockType, int>
        {
            { StockType.BasicPlate, 10 }
        };

        public int DayNum { get; set; } = 1;

        public int MinCustomers { get; set; } = 30;

        public float MinTimeBetweenSpawn { get; set; } = 0.2f;

        public float ShopperThinkTime { get; set; } = 2f;

        public float ShopperImpatienceTime { get; set; } = 5f;
        
        public DayStats DayStats { get; set; }

    }
}
