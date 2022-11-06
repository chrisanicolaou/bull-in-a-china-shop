using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        [field: SerializeField]
        public int Cash { get; set; } = 500;

        public Dictionary<StockType, int> AvailableStock { get; set; } = new Dictionary<StockType, int>
        {
            { StockType.BasicPlate, 10 }
        };

        public int DayNum { get; set; } = 2;

        [field: SerializeField]
        public int MinCustomers { get; set; } = 1000;

        [field: SerializeField]
        public float MinTimeBetweenSpawn { get; set; } = 0.1f;

        [field: SerializeField]
        public float ShopperThinkTime { get; set; } = 1f;

        [field: SerializeField]
        public float ShopperImpatienceTime { get; set; } = 3f;

        [field: SerializeField]
        public int NumOfDaysBetweenBullEncounters { get; set; } = 3;
        
        public DayStats DayStats { get; set; }

    }
}
