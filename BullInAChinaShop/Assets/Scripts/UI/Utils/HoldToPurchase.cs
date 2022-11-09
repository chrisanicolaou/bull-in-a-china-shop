using System.Collections;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.PurchasableItems;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class HoldToPurchase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private bool _isHovering;

        private bool _isHoldPurchasing;

        private float _initialHoldPurchaseDelay = 0.4f;

        private float _rampUpSpeed = 0.8f;

        private float _maxPurchaseSpeed = 0.05f;
        
        public IPurchasableItem Item { get; set; }

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
            var delay = _initialHoldPurchaseDelay;
            while (_isHoldPurchasing)
            {
                Item.PurchaseItem();
                delay *= _rampUpSpeed;
                yield return new WaitForSeconds(Mathf.Max(delay, _maxPurchaseSpeed));
            }
        }
    }
}