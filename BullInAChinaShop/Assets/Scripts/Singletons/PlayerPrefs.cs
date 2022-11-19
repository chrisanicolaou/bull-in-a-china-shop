using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class PlayerPrefs : Singleton<PlayerPrefs>
    {
        public int MasterVol { get; set; } = 100;
    }
}