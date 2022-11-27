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

        private ShopperReview _review;

        private Image _img;

        private ImpatienceBar _impatienceBar;

        private IEnumerator _impatienceCoroutine;

        private IEnumerator _loiterCoroutine;

        private IEnumerator _joiningQueueCoroutine;

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

        private Sequence _moveAlongSeq;

        private AudioSource _audioSource;

        [SerializeField]
        private ShopperSfx _sfx = new ShopperSfx();

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            _audioSource = GetComponent<AudioSource>();
            Rect.SetSiblingIndex(1);

            _img = GetComponent<Image>();

            _enterShopCoroutine = EnterShop();
            _loiterCoroutine = Loiter();
            _joiningQueueCoroutine = WalkToQueue();
            _impatienceCoroutine = ImpatienceTimer();
            _thinkingCoroutine = Think();

            _animatorIds = new[] { IsIdle, IsAnnoyed, IsWalkingSide, IsWalkingAway, IsWalkingForward };
            _defaultIdle = IsIdle;
            _review = new ShopperReview(name[..name.IndexOf("(", StringComparison.Ordinal)]);
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
            yield return null;

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
            yield return null;

            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);

            yield return seq.WaitForCompletion();

            Animate(_defaultIdle);

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(Controller.CloseDoorCoroutine);

            var shouldLoiter = Random.Range(1, 5) < 4;
            if (shouldLoiter || !Controller.CanJoinQueue())
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
                    StartCoroutine(_joiningQueueCoroutine);
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
                StartCoroutine(_joiningQueueCoroutine);
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

            if (_isLeaving) yield break;

            _defaultIdle = IsAnnoyed;
            Animate(_defaultIdle);
            _impatienceBar = GetComponentInChildren<ImpatienceBar>();
            _impatienceBar.GetImpatient(waitTime / 2);

            yield return new WaitForSeconds(waitTime / 2);

            StopCoroutine(_loiterCoroutine);

            StartCoroutine(LeaveInAHuff());
        }

        public IEnumerator Think()
        {
            if (_isLeaving) yield break;
            if (_impatienceBar != null) _impatienceBar.StopImpatienceBar();
            _isBeingServed = true;
            StopCoroutine(_impatienceCoroutine);
            StopCoroutine(_loiterCoroutine);

            _defaultIdle = IsIdle;
            Animate(_defaultIdle);
            if (_sfx.Thinking != null) _audioSource.PlayOneShot(_sfx.Thinking);
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
            
            if (_sfx.Decided != null) _audioSource.PlayOneShot(_sfx.Decided);

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
            _review.RequestedStock = stock;
            var baseAmount = GameManager.Instance.ShopperPurchaseAmount;
            var amount = Random.Range(baseAmount, Mathf.Max(3, (int)(baseAmount * 1.1)));
            StartCoroutine(Controller.RequestStock(stock, amount) ? LeaveHappily() : LeaveInAHuff());
        }

        private IEnumerator LeaveInAHuff()
        {
            if (_sfx.Angry != null) _audioSource.PlayOneShot(_sfx.Angry);
            _review.Type = ReviewType.Unhappy;
            _review.GenerateReviewText();
            Controller.DayStats.Reviews.Add(_review);
            if (Controller.ShopperQueue.Contains(this)) Controller.Remove(this);
            StartCoroutine(ExitShop());
            yield break;
        }

        private IEnumerator LeaveHappily()
        {
            _review.Type = ReviewType.Happy;
            _review.GenerateReviewText();
            Controller.DayStats.Reviews.Add(_review);
            Controller.Remove(this);

            StartCoroutine(ExitShop());
            yield break;
        }

        private IEnumerator ExitShop()
        {
            if (_impatienceBar != null) _impatienceBar.StopImpatienceBar();
            _isLeaving = true;
            StopCoroutine(_thinkingCoroutine);
            StopCoroutine(_joiningQueueCoroutine);

            Animate(IsWalkingAway);
            yield return null;

            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);
            yield return seq.WaitForCompletion();

            Animate(null);
            yield return null;

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
            StartCoroutine(Controller.CloseDoorCoroutine);
            Animate(IsWalkingSide);
            Rect.SetSiblingIndex(1);
            Mover.MoveTo(Rect, ShopLocation.OutsideStart)
                .OnComplete(() => Destroy(gameObject));
        }

        public void MoveAlong(int index)
        {
            _moveAlongSeq?.Kill();
            _moveAlongSeq = null;

            if (_isLeaving) return;
            StopCoroutine(_joiningQueueCoroutine);

            if (!_isGrowingImpatient)
            {
                StartCoroutine(_impatienceCoroutine);
            }

            if (index == 0)
            {
                StopCoroutine(_impatienceCoroutine);
                _defaultIdle = IsIdle;
            }

            Animate(IsWalkingSide);

            var posToWalkTo = Mover.CalculateQueuePosition(index);

            _moveAlongSeq = Mover.MoveTo(Rect, posToWalkTo);

            _moveAlongSeq.OnComplete(() =>
            {
                _moveAlongSeq = null;
                if (_isLeaving) return;
                Animate(_defaultIdle);
                if (index != 0 || _isBeingServed) return;
                StartCoroutine(_thinkingCoroutine);
            });
        }

        private void Animate(int? id)
        {
            if (id == null)
            {
                _animator.enabled = false;
                return;
            }

            var animId = (int)id;

            foreach (var animatorId in _animatorIds)
            {
                _animator.SetBool(animatorId, animatorId == animId);
            }

            if (!_animator.enabled) _animator.enabled = true;
        }
    }
}