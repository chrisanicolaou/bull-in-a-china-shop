using System;
using System.Collections;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.PurchasableItems.Stock
{
    public abstract class BaseStock : PurchasableItem, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public abstract StockType Type { get; set; }

        private TextMeshProUGUI _costText;

        private bool _isHovering;

        private bool _isHoldPurchasing;

        private float _initialHoldPurchaseDelay = 0.3f;

        private float _rampUpSpeed = 0.8f;

        private float _maxPurchaseSpeed = 0.05f;

        public override void Start()
        {
            _costText = GetComponentInChildren<TextMeshProUGUI>();
            base.Start();
        }

        public override void UpdatePurchasable(Dictionary<string, object> dictionary)
        {
            _costText.text = Cost > GameManager.Instance.Cash ? $"<color=\"red\">{Cost}</color>" : $"<color=\"green\">{Cost}</color>";
        }

        public override void OnPurchase()
        {
            if (GameManager.Instance.AvailableStock.ContainsKey(Type))
            {
                GameManager.Instance.AvailableStock[Type]++;
            }
            else
            {
                GameManager.Instance.AvailableStock[Type] = 1;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _isHoldPurchasing = false;
            StopCoroutine(nameof(OnHoldPurchase));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isHovering) return;
            _isHoldPurchasing = true;
            StartCoroutine(nameof(OnHoldPurchase));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHoldPurchasing = false;
            StopCoroutine(nameof(OnHoldPurchase));
        }

        private IEnumerator OnHoldPurchase()
        {
            yield return new WaitForSeconds(_initialHoldPurchaseDelay);
            var delay = _initialHoldPurchaseDelay;
            while (_isHoldPurchasing)
            {
                PurchaseItem();
                delay *= _rampUpSpeed;
                yield return new WaitForSeconds(Mathf.Max(delay, _maxPurchaseSpeed));
            }
        }
    }
}
