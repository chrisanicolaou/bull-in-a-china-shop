using CharaGaming.BullInAChinaShop.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class StockBuilder
    {
        private bool _isPurchasable;

        private float _scale = 1f;

        private Vector3 _rotation;

        private bool _shouldClean;

        private Transform _parent;

        private BaseStock _stock;

        public StockBuilder()
        {
            
        }

        public StockBuilder SetStock(BaseStock stock)
        {
            _stock = stock;
            return this;
        }

        public StockBuilder SetParent(Transform parent)
        {
            _parent = parent;
            return this;
        }
        
        public StockBuilder SetScale(float scale)
        {
            _scale = scale;
            return this;
        }
        
        public StockBuilder SetRotation(Vector3 rotation)
        {
            _rotation = rotation;
            return this;
        }

        public StockBuilder AsPurchasable(bool purchasable = true)
        {
            _isPurchasable = purchasable;
            return this;
        }

        public StockBuilder CleanOnBuild(bool shouldClean = true)
        {
            _shouldClean = shouldClean;
            return this;
        }

        public GameObject Build()
        {
            var stockObj = Object.Instantiate(Resources.Load<GameObject>("Stock"), Vector3.zero, Quaternion.identity);
            stockObj.transform.SetParent(_parent);
            stockObj.transform.localScale = new Vector3(_scale, _scale, _scale);
            stockObj.transform.rotation = Quaternion.Euler(_rotation);

            var stockImg = stockObj.FindComponentInChildWithTag<Image>("StockImage");
            stockImg.sprite = Resources.Load<Sprite>(_stock.SpriteFilePath);

            if (_isPurchasable)
            {
                var upgradeBox = stockObj.FindComponentInChildWithTag<Image>("UpgradeBox");
                upgradeBox.enabled = true;
            }

            if (_shouldClean)
            {
                _stock = null;
                _parent = null;
                _scale = default;
                _rotation = default;
                _isPurchasable = default;
            }

            return stockObj;
        }
        
    }
}