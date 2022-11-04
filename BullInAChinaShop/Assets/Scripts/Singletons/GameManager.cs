using System.Collections.Generic;
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

        public int MinCustomers { get; set; } = 5;

        public float MinTimeBetweenSpawn { get; set; } = 5f;

        public float ShopperThinkTime { get; set; } = 5f;

        public float ShopperImpatienceTime { get; set; } = 12f;

    }
}
