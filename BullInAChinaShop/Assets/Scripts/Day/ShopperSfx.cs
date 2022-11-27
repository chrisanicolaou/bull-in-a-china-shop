using System;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    [Serializable]
    public class ShopperSfx
    {
        [field: SerializeField]
        public AudioClip Thinking { get; set; }
        
        [field: SerializeField]
        public AudioClip Decided { get; set; }
        
        [field: SerializeField]
        public AudioClip Angry { get; set; }
    }
}