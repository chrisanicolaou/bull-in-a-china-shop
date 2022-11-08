using System;
using CharaGaming.BullInAChinaShop.Enums;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Utils
{
    [Serializable]
    public class ShopPosition
    {
        [field: SerializeField]
        public ShopLocation Location { get; set; }
        
        [field: SerializeField]
        public Vector2 VectorPos { get; set; }
        
        [field: SerializeField]
        public float Scale { get; set; }
    }
}