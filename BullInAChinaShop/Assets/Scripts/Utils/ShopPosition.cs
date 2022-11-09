using System;
using CharaGaming.BullInAChinaShop.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Utils
{
    
    [Serializable]
    public class ShopPosition
    {
        [field: SerializeField]
        public ShopLocation Location { get; set; }
        
        [SerializeField]
        private bool _singleLocation = true;

        [SerializeField]
        private Vector2 _vectorPos;

        [SerializeField]
        private float _scale;
        
        public (Vector2 pos, float scale) PosAndScale => _singleLocation ? (_vectorPos, _scale) : GetRandomSpotInRange();

        [field: SerializeField]
        public Vector2 MinVectorPos { get; set; }
        
        [field: SerializeField]
        public Vector2 MaxVectorPos { get; set; }
        
        [field: SerializeField]
        public float MinScale { get; set; }
        
        [field: SerializeField]
        public float MaxScale { get; set; }

        public (Vector2 pos, float scale) GetRandomSpotInRange()
        {
            var xDiff = MaxVectorPos.x - MinVectorPos.x;
            var yDiff = MaxVectorPos.y - MinVectorPos.y;
            var scaleDiff = MaxScale - MinScale;
            var randomXPos = Random.Range(MinVectorPos.x, MaxVectorPos.x);

            var proportion = Math.Abs(randomXPos - MinVectorPos.x) < 0.001f ? 0 : (randomXPos - MinVectorPos.x) / xDiff;

            var randomYPos = (proportion * yDiff) + MinVectorPos.y;
            var scale = (proportion * scaleDiff) + MinScale;

            return (new Vector2(randomXPos, randomYPos), scale);
        }
    }
}