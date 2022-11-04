using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        
        public int Cash { get; set; } = 500;

        public Dictionary<StockType, int> AvailableStock { get; set; } = new Dictionary<StockType, int>();

    }
}
