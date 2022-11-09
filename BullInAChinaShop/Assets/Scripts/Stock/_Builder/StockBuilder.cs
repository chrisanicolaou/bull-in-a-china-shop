﻿using CharaGaming.BullInAChinaShop.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Stock
{
    public class StockBuilder
    {
        private bool _isPurchasable;

        private float _scale = 1f;

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

        public StockBuilder AsPurchasable(bool purchasable = true)
        {
            _isPurchasable = purchasable;
            return this;
        }

        public GameObject Build()
        {
            var stockObj = Object.Instantiate(Resources.Load<GameObject>("Stock"), Vector3.zero, Quaternion.identity);
            stockObj.transform.SetParent(_parent);
            stockObj.transform.localScale = new Vector3(_scale, _scale, _scale);

            var stockImg = stockObj.FindComponentInChildWithTag<Image>("StockImage");
            stockImg.sprite = Resources.Load<Sprite>(_stock.SpriteFilePath);

            if (_isPurchasable)
            {
                var upgradeBox = stockObj.FindComponentInChildWithTag<Image>("UpgradeBox");
                upgradeBox.enabled = true;
            }

            return stockObj;
        }
        
    }
}