using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class Shopper : MonoBehaviour
    {
        public DayController Controller { get; set; }

        public CharacterMover Mover { get; set; }

        [SerializeField]
        private GameObject _thoughtBubble;

        [SerializeField]
        private float _minLoiterTime;

        [SerializeField]
        private float _maxLoiterTime;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private Sprite _idleSprite;

        [SerializeField]
        private Sprite _facingForwardSprite;

        [SerializeField]
        private Sprite _facingAwaySprite;

        [SerializeField]
        private Sprite _facingSideSprite;

        public RectTransform Rect { get; set; }

        private Image _img;

        private IEnumerator _impatienceCoroutine;

        private IEnumerator _loiterCoroutine;

        private IEnumerator _joiningQueueCoroutine;

        private IEnumerator _walkToQueueCoroutine;

        private bool _isInShop;
        
        private bool _isBeingServed;

        private bool _isGrowingImpatient;
        
        private bool _isLeaving;

        private IEnumerator _enterShopCoroutine;

        private IEnumerator _thinkingCoroutine;
        
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int IsAnnoyed = Animator.StringToHash("isAnnoyed");
        private static readonly int IsWalkingForward = Animator.StringToHash("isWalkingForward");
        private static readonly int IsWalkingAway = Animator.StringToHash("isWalkingAway");
        private static readonly int IsWalkingSide = Animator.StringToHash("isWalkingSide");

        private int[] _animatorIds;

        private int _defaultIdle;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            Rect.SetSiblingIndex(1);

            _img = GetComponent<Image>();
            
            _enterShopCoroutine = EnterShop();
            _loiterCoroutine = Loiter();
            _joiningQueueCoroutine = WalkToQueue();
            _impatienceCoroutine = ImpatienceTimer();
            _walkToQueueCoroutine = WalkToQueue();
            _thinkingCoroutine = Think();

            _animatorIds = new[] { IsIdle, IsAnnoyed, IsWalkingForward, IsWalkingAway, IsWalkingSide };
            _defaultIdle = IsIdle;
        }

        private void Update()
        {
            if (!_isInShop) return;

            var siblingIndex = Rect.parent.childCount - 2;

            if (_isBeingServed) return;

            for (int i = 0; i < Rect.parent.childCount; i++)
            {
                var sibling = Rect.parent.GetChild(i);
                if (!sibling.CompareTag("Shopper")) continue;
                if (sibling.localScale.x >= Rect.localScale.x) siblingIndex--;
            }

            Rect.SetSiblingIndex(siblingIndex);
        }

        public IEnumerator ApproachShop()
        {
            Animate(IsWalkingSide);
            var seq = Mover.MoveTo(Rect, ShopLocation.OutsideDoor);

            yield return seq.WaitForCompletion();
            
            Animate(null);
            _img.sprite = _facingForwardSprite;
            _img.SetNativeSize();

            var canEnter = Controller.RequestShopEntry();

            if (!canEnter)
            {
                yield return new WaitForSeconds(0.5f);
                WalkAway();
                yield break;
            }

            yield return new WaitUntil(() => Controller.IsDoorOpen);

            StartCoroutine(_enterShopCoroutine);
        }

        public IEnumerator EnterShop()
        {
            _isInShop = true;
            
            Animate(IsWalkingForward);
            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);

            yield return seq.WaitForCompletion();
            
            Animate(_defaultIdle);
            
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(Controller.CloseDoorCoroutine);

            var shouldLoiter = Random.Range(1, 5) < 4;
            if (shouldLoiter)
            {
                StartCoroutine(_loiterCoroutine);
            }
            else
            {
                StartCoroutine(_impatienceCoroutine);
                StartCoroutine(_joiningQueueCoroutine);
            }
        }

        public IEnumerator Loiter()
        {
            var loiterPoints = new List<ShopLocation>() { ShopLocation.LoiterOne, ShopLocation.LoiterTwo, ShopLocation.LoiterThree };
            var totalLoiterTime = Random.Range(_minLoiterTime, _maxLoiterTime);
            var numOfPointsToVisit = Random.Range(1, 4);
            var intervalLoiterTime = totalLoiterTime / numOfPointsToVisit;

            for (int i = 0; i < numOfPointsToVisit; i++)
            {
                if (Controller.CanJoinQueue() && _isGrowingImpatient)
                {
                    StartCoroutine(_walkToQueueCoroutine);
                    yield break;
                }

                var randomLoiterPoint = Random.Range(0, loiterPoints.Count);
                
                Animate(IsWalkingSide);
                var seq = Mover.MoveTo(Rect, loiterPoints[randomLoiterPoint]);

                yield return seq.WaitForCompletion();
                
                Animate(_defaultIdle);
                yield return new WaitForSeconds(intervalLoiterTime);
            }

            if (Controller.CanJoinQueue())
            {
                StartCoroutine(_walkToQueueCoroutine);
            }
            else
            {
                StartCoroutine(_impatienceCoroutine);
                StartCoroutine(_loiterCoroutine);
            }
        }

        public IEnumerator WalkToQueue()
        {
            Controller.ShopperQueue.Add(this);
            
            Animate(IsWalkingForward);
            var seq = Mover.JoinQueue(Rect);
            yield return seq.WaitForCompletion();
            var positionInQueue = Controller.ShopperQueue.IndexOf(this);

            if (positionInQueue == 0 && !_isBeingServed)
            {
                StartCoroutine(_thinkingCoroutine);
            }
            else
            {
                if (!_isGrowingImpatient)
                {
                    StartCoroutine(_impatienceCoroutine);
                }
            }
            
            Animate(_defaultIdle);
        }

        public IEnumerator ImpatienceTimer()
        {
            _isGrowingImpatient = true;
            
            var waitTime = GameManager.Instance.ShopperImpatienceTime;
            
            yield return new WaitForSeconds(waitTime / 2);
            
            _defaultIdle = IsAnnoyed;
            Animate(_defaultIdle);

            yield return new WaitForSeconds(waitTime / 2);

            StopCoroutine(_loiterCoroutine);

            StartCoroutine(LeaveInAHuff());
        }

        public IEnumerator Think()
        {
            _isBeingServed = true;
            StopCoroutine(_impatienceCoroutine);
            StopCoroutine(_loiterCoroutine);
            
            _defaultIdle = IsIdle;
            Animate(_defaultIdle);
            _thoughtBubble.SetActive(true);
            yield return new WaitForSeconds(GameManager.Instance.ShopperThinkTime);

            var thoughtBubbleAnim = _thoughtBubble.GetComponent<Animator>();
            thoughtBubbleAnim.SetBool("isRequestingStock", true);
            var stock = GameManager.Instance.AvailableStock[Random.Range(0, GameManager.Instance.AvailableStock.Count)];

            var stockObj = new StockBuilder().SetParent(_thoughtBubble.transform).SetScale(0f).SetStock(stock).Build();
            stockObj.transform.DOScale(0.35f, 0.2f);
            var stockRect = stockObj.GetComponent<RectTransform>();
            stockRect.anchorMin = new Vector2(0.5f, 0.5f);
            stockRect.anchorMax = new Vector2(0.5f, 0.5f);
            stockRect.anchoredPosition = stockRect.anchorMin;

            yield return new WaitForSeconds(GameManager.Instance.ShopperServeTime);
            
            thoughtBubbleAnim.SetBool("hasRequestedStock", true);

            yield return new WaitForSeconds(0.3f);
            stockObj.transform.DOScale(0f, 0.1f);
            _thoughtBubble.transform.DOScale(0f, 0.5f).OnComplete(() =>
            {
                _thoughtBubble.SetActive(false);
                PurchaseStock(stock);
            });
        }

        private void PurchaseStock(BaseStock stock)
        {
            _isBeingServed = false;
            StartCoroutine(Controller.RequestStock(stock, 2) ? LeaveHappily() : LeaveInAHuff());
        }

        private IEnumerator LeaveInAHuff()
        {
            Controller.DayStats.UnhappyShoppers.Add(new UnhappyShopper
            {
                Sprite = GetComponent<Image>().sprite,
                Review = "I had a bad experience! :("
            });
            if (Controller.ShopperQueue.Contains(this)) Controller.Remove(this);
            StartCoroutine(ExitShop());
            yield break;
        }

        private IEnumerator LeaveHappily()
        {
            Controller.Remove(this);

            StartCoroutine(ExitShop());
            yield break;
        }

        private IEnumerator ExitShop()
        {
            _isLeaving = true;
            Animate(IsWalkingAway);
            
            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);
            yield return seq.WaitForCompletion();

            Animate(null);
            _img.sprite = _facingAwaySprite;
            _img.SetNativeSize();
            
            StartCoroutine(Controller.OpenDoor());
            yield return new WaitUntil(() => Controller.IsDoorOpen);


            _isInShop = false;
            Animate(IsWalkingAway);
            seq = Mover.MoveTo(Rect, ShopLocation.OutsideDoor);

            yield return seq.WaitForCompletion();

            WalkAway();
        }

        private void WalkAway()
        {
            Animate(IsWalkingSide);
            Rect.SetSiblingIndex(1);
            Mover.MoveTo(Rect, ShopLocation.OutsideStart)
                .OnComplete(() => Destroy(gameObject));
        }

        public void MoveAlong(int index)
        {
            if (_isLeaving) return;
            StopCoroutine(_joiningQueueCoroutine);
            
            if (!_isGrowingImpatient)
            {
                StartCoroutine(_impatienceCoroutine);
            }
            
            Animate(IsWalkingSide);

            var posToWalkTo = Mover.CalculateQueuePosition(index);

            var seq = Mover.MoveTo(Rect, posToWalkTo);

            seq.OnComplete(() =>
            {
                Animate(_defaultIdle);
                if (index != 0 || _isBeingServed) return;
                StartCoroutine(_thinkingCoroutine);
            });
        }

        private void Animate(int? id)
        {
            foreach (var animatorId in _animatorIds)
            {
                _animator.SetBool(animatorId, false);
            }

            if (id == null) return;
            
            _animator.SetBool((int)id, true);
        }
    }
}