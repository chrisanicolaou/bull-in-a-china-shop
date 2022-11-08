using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
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
        
        public RectTransform Rect { get; set; }

        private IEnumerator _impatienceCoroutine;

        private IEnumerator _loiterCoroutine;

        private IEnumerator _joiningQueueCoroutine;

        private bool _isInShop;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            Rect.SetSiblingIndex(1);
        }

        private void Update()
        {
            if (!_isInShop) return;

            var siblingIndex = Rect.parent.childCount - 3;

            for (int i = 0; i < Rect.parent.childCount; i++)
            {
                var sibling = Rect.parent.GetChild(i);
                if (!sibling.CompareTag("Shopper")) continue;
                if (sibling.localScale.x > Rect.localScale.x) siblingIndex--;
            }
            
            Rect.SetSiblingIndex(siblingIndex);
            
        }

        public IEnumerator ApproachShop()
        {
            var seq = Mover.MoveTo(Rect, ShopLocation.OutsideDoor);

            yield return seq.WaitForCompletion();

            var canEnter = Controller.RequestShopEntry();
            
            if (!canEnter) yield break;

            yield return new WaitUntil(() => Controller.IsDoorOpen);

            StartCoroutine(EnterShop());
        }

        public IEnumerator EnterShop()
        {
            _isInShop = true;
            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);

            yield return seq.WaitForCompletion();
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(Controller.CloseDoor());

            var shouldLoiter = Random.Range(1, 5) < 4;
            if (shouldLoiter)
            {
                _loiterCoroutine = Loiter();
                StartCoroutine(_loiterCoroutine);
            }
            else
            {
                _joiningQueueCoroutine = WalkToQueue();
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
                if (_impatienceCoroutine != null && Controller.CanJoinQueue())
                {
                    StopCoroutine(_impatienceCoroutine);
                    StartCoroutine(WalkToQueue());
                    yield break;
                }
                var randomLoiterPoint = Random.Range(0, loiterPoints.Count);
                var seq = Mover.MoveTo(Rect, loiterPoints[randomLoiterPoint]);
                
                yield return seq.WaitForCompletion();
                yield return new WaitForSeconds(intervalLoiterTime);
            }

            if (Controller.CanJoinQueue())
            {
                StartCoroutine(WalkToQueue());
            }
            else
            {
                if (_impatienceCoroutine == null)
                {
                    _impatienceCoroutine = ImpatienceTimer();
                    StartCoroutine(_impatienceCoroutine);
                }
                StartCoroutine(_loiterCoroutine);
            }
        }

        public IEnumerator WalkToQueue()
        {
            Controller.ShopperQueue.Add(this);
            var seq = Mover.JoinQueue(Rect);

            yield return seq.WaitForCompletion();

            var positionInQueue = Controller.ShopperQueue.IndexOf(this);

            if (positionInQueue == 0)
            {
                StartCoroutine(Think());
            }
            else
            {
                if (_impatienceCoroutine == null)
                {
                    _impatienceCoroutine = ImpatienceTimer();
                    StartCoroutine(_impatienceCoroutine);
                }
                // Play idle in queue animation
            }
        }

        public IEnumerator ImpatienceTimer()
        {
            var waitTime = GameManager.Instance.ShopperImpatienceTime;

            yield return new WaitForSeconds(waitTime);
            
            StopCoroutine(_loiterCoroutine);

            StartCoroutine(LeaveInAHuff());
        }
        
        public IEnumerator Think()
        {
            if (_impatienceCoroutine != null)
            {
                StopCoroutine(_impatienceCoroutine);
            }
            _thoughtBubble.SetActive(true);
            yield return new WaitForSeconds(GameManager.Instance.ShopperThinkTime);
            _thoughtBubble.SetActive(false);
            PurchaseStock();
        }
        
        private void PurchaseStock()
        {
            StartCoroutine(Controller.RequestStock(StockType.OldPlate, 2) ? LeaveHappily() : LeaveInAHuff());
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
            var seq = Mover.MoveTo(Rect, ShopLocation.InsideDoor);
            yield return seq.WaitForCompletion();

            StartCoroutine(Controller.OpenDoor());

            yield return new WaitUntil(() => Controller.IsDoorOpen);


            _isInShop = false;
            seq = Mover.MoveTo(Rect, ShopLocation.OutsideDoor);

            yield return seq.WaitForCompletion();

            Rect.SetSiblingIndex(1);
            seq = Mover.MoveTo(Rect, ShopLocation.OutsideStart);
            
            yield return seq.WaitForCompletion();
            
            Destroy(gameObject);
        }
        
        public void MoveAlong(int index)
        {
            if (_joiningQueueCoroutine != null) StopCoroutine(_joiningQueueCoroutine);
            
            var posToWalkTo = Mover.CalculateQueuePosition(index);

            var seq = Mover.MoveTo(Rect, posToWalkTo);

            seq.OnComplete(() =>
            {
                if (index != 0) return;
                if (_impatienceCoroutine != null) StopCoroutine(_impatienceCoroutine);
                StartCoroutine(Think());
            });
        }
    }
}