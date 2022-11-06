using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class Shopper : MonoBehaviour
    {
        public DayController Controller { get; set; }

        public bool IsLeaving { get; set; }

        [SerializeField]
        private GameObject _thoughtBubble;

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

        private bool _isServed;

        private bool _forceStopIdle;

        private Dictionary<string, object> _defaultEventMessage;

    private Dictionary<string, object> DefaultEventMessage
    {
        get { return _defaultEventMessage ??= new Dictionary<string, object> { { "shopper", this } }; }
    }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rect.anchoredPosition = _startPosition;
            _rect.localScale = new Vector3(_startScale, _startScale, _startScale);
            _rect.SetSiblingIndex(1);
            GameEventsManager.Instance.AddListener(GameEvent.ShopperDeniedEntry, OnRejectedEntry);
            GameEventsManager.Instance.AddListener(GameEvent.DoorOpened, JoinQueue);
        }

        public void WalkToDoor()
        {
            transform.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);
            _rect.DOAnchorPos(_doorPosition, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() => GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperRequestingEntry, DefaultEventMessage));
        }

        public void OnRejectedEntry(Dictionary<string, object> message)
        {
            if (!IsTargetedShopper(message)) return;
            
            var seq = DOTween.Sequence();
            seq.AppendInterval(1.5f);
            seq.Append(_rect.DOAnchorPos(_startPosition, _walkSpeed));
            seq.OnComplete(() => Destroy(gameObject));
        }

        public void JoinQueue(Dictionary<string, object> message)
        {
            if (!IsTargetedShopper(message)) return;
            
            var siblingIndex = _rect.parent.childCount - (2 + Controller.ShopperQueue.Count);
            _rect.SetSiblingIndex(siblingIndex);
            
            var posToWalkTo = new Vector3(_tillPosition.x - _queuePositionXOffset * (Controller.ShopperQueue.Count - 1), _tillPosition.y, _tillPosition.z);
            
            transform.DOScale(1f, _walkSpeed).SetEase(Ease.Linear);
            _rect.DOAnchorPos(posToWalkTo, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() => GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperQueued, DefaultEventMessage));
            // .OnComplete(() => { StartCoroutine(Controller.ShopperQueue.Count == 1 ? nameof(Think) : nameof(Idle)); });
        }

        public void MoveAlong(int index)
        {
            var posToWalkTo = new Vector3(_tillPosition.x - _queuePositionXOffset * (index), _tillPosition.y, _tillPosition.z);
            _rect.DOAnchorPos(posToWalkTo, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (index != 0) return;
                    ForceStopCoroutines();
                    StartCoroutine(nameof(Think));
                });
        }

        public IEnumerator Idle()
        {
            // Start idle animation
            var waitTime = GameManager.Instance.ShopperImpatienceTime;
            
            yield return new WaitForSeconds(waitTime);
            if (!IsLeaving && !_forceStopIdle) LeaveInAHuff();
        }

        private void LeaveInAHuff()
        {
            IsLeaving = true;
            _thoughtBubble.SetActive(false);
            ForceStopCoroutines();
            Controller.DayStats.UnhappyShoppers.Add(new UnhappyShopper
            {
                Sprite = GetComponent<Image>().sprite,
                Review = "I had a bad experience! :("
            });
            WalkOutShop();
        }

        private void WalkOutShop()
        {
            IsLeaving = true;
            _isServed = true;
            GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperLeaving, DefaultEventMessage);
            
            // Controller.OnShopperExit(this);
            var siblingIndex = _rect.parent.childCount - (2 + Controller.ShopperQueue.Count + 1);
            _rect.SetSiblingIndex(siblingIndex);
            transform.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);

            var seq = DOTween.Sequence();

            seq.Append(_rect.DOAnchorPos(_doorPosition, _walkSpeed)
                .OnComplete(() => _rect.SetSiblingIndex(1)));
            seq.Append(_rect.DOAnchorPos(_startPosition, _walkSpeed));
            seq.OnComplete(() =>
            {
                DestroyImmediate(gameObject);
            });
        }

        private void PurchaseStock()
        {
            if (!_isServed && Controller.RequestStock(StockType.BasicPlate, 2))
            {
                _isServed = true;
                LeaveHappily();
                return;
            }

            if (!_isServed)
            {
                LeaveInAHuff();
            }

            _isServed = true;
        }

        private void LeaveHappily()
        {
            WalkOutShop();
        }

        public IEnumerator Think()
        {
            if (_isServed) yield break;
            _thoughtBubble.SetActive(true);
            yield return new WaitForSeconds(GameManager.Instance.ShopperThinkTime);
            _thoughtBubble.SetActive(false);
            PurchaseStock();
        }

        public void ForceStopCoroutines()
        {
            _forceStopIdle = true;
            StopAllCoroutines();
        }
        
        private bool IsTargetedShopper(Dictionary<string, object> message)
        {
            var shopper = ShopperUtils.GetShopperFromMessage(message);
            return shopper != null && shopper.Equals(this);
        }

        private void OnDestroy()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.ShopperDeniedEntry, OnRejectedEntry);
            GameEventsManager.Instance.RemoveListener(GameEvent.DoorOpened, JoinQueue);
        }
    }
}