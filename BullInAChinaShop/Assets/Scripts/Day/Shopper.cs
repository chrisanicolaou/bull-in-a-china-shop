using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using DG.Tweening;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class Shopper : MonoBehaviour
    {
        public DayController Controller { get; set; }

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _startScale;

        [SerializeField]
        private float _walkSpeed = 1f;

        [SerializeField]
        private Vector3 _startPosition;

        [SerializeField]
        private Vector3 _doorPosition;

        [SerializeField]
        private Vector3 _tillPosition;

        [SerializeField]
        private float _queuePositionXOffset;

        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rect.anchoredPosition = _startPosition;
            _rect.localScale = new Vector3(_startScale, _startScale, _startScale);
            _rect.SetSiblingIndex(1);
        }

        public void WalkToDoor()
        {
            transform.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);
            _rect.DOAnchorPos(_doorPosition, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Controller.RequestShopEntry(this);
                });
        }

        public void WalkToTill(int positionInQueue)
        {
            _rect.SetSiblingIndex(6);
            var posToWalkTo = new Vector3(_tillPosition.x - _queuePositionXOffset * (positionInQueue - 1), _tillPosition.y, _tillPosition.z);
            transform.DOScale(1f, _walkSpeed).SetEase(Ease.Linear);
            _rect.DOAnchorPos(posToWalkTo, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (positionInQueue == 1)
                    {
                        PurchaseStock();
                    }
                    else
                    {
                        StartCoroutine(nameof(Idle));
                    }
                });
        }

        private IEnumerator Idle()
        {
            // Start idle animation
            yield return new WaitForSeconds(GameManager.Instance.ShopperImpatienceTime);
            if (this == Controller.ShopperQueue.Peek())
            {
                PurchaseStock();
            }
            else
            {
                LeaveInAHuff();
            }
        }

        private void LeaveInAHuff(bool alreadyDequeued = false)
        {
            if (!alreadyDequeued)
            {
                Controller.ShopperQueue = new Queue<Shopper>(Controller.ShopperQueue.Where(s => s != this));
            }

            WalkOutShop();
        }

        private void WalkOutShop()
        {
            transform.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);

            var seq = DOTween.Sequence();

            seq.Append(_rect.DOAnchorPos(_doorPosition, _walkSpeed)
                .OnComplete(() => _rect.SetSiblingIndex(1)));
            seq.Append(_rect.DOAnchorPos(_startPosition, _walkSpeed));
            seq.OnComplete(() => Destroy(gameObject));
        }

        private void PurchaseStock()
        {
            Controller.ShopperQueue.Dequeue();
            if (Controller.RequestStock(StockType.BasicPlate, 2))
            {
                LeaveHappily();
                return;
            }
            
            LeaveInAHuff(true);
        }

        private void LeaveHappily()
        {
            WalkOutShop();
        }

        public void Think()
        {
            
        }
    }
}
