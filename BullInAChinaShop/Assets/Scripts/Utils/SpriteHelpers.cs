using System.Linq;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Utils
{
    public static class SpriteHelpers
    {
        public static Sprite LoadFromSheet(string sheetPath, string spriteName)
        {
            var sprites = Resources.LoadAll<Sprite>(sheetPath);
            
            return sprites.FirstOrDefault(s => s.name == spriteName);
        }
    }
}