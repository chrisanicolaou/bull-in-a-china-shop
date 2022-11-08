using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class DayController : MonoBehaviour
    {
        public List<Shopper> ShopperQueue { get; set; } = new List<Shopper>();

        public DayStats DayStats { get; } = new DayStats();
        
        [SerializeField]
        private Button _startDayButton;

        [SerializeField]
        private GameObject _shopperPrefab;

        [SerializeField]
        private GameObject _bullEncounterPrefab;

        [SerializeField]
        private Sprite[] _shopperSprites;

        [SerializeField]
        private Transform _shopperSpawnCanvas;

        [SerializeField]
        private Animator _doorAnimator;
        
        [field: SerializeField]
        public CharacterMover Mover { get; set; }

        private int _remainingCustomers;

        public bool IsDoorOpen { get; set; }

        private IEnumerator _doorOpenCoroutine;
        
        private static readonly int ShouldOpen = Animator.StringToHash("shouldOpen");

        private void Start()
        {
            if (GameManager.Instance.DayNum == 1)
            {
                var bullEncounter = Instantiate(_bullEncounterPrefab).GetComponent<BullEncounter>();
                bullEncounter.Controller = this;
                return;
            }
            EnableStartAndUpgradeButtons();
        }

        public void EnableStartAndUpgradeButtons()
        {
            _startDayButton.onClick.AddListener(() =>
            {
                StartCoroutine(nameof(StartDay));
                Destroy(_startDayButton.gameObject);
            });
            _remainingCustomers = Random.Range(GameManager.Instance.MinCustomers, GameManager.Instance.MinCustomers + 4);
        }

        private IEnumerator StartDay()
        {
            while (_remainingCustomers > 0)
            {
                var shopper = LoadShopper();
                StartCoroutine(shopper.ApproachShop());
                yield return new WaitForSeconds(Random.Range(GameManager.Instance.MinTimeBetweenSpawn, GameManager.Instance.MinTimeBetweenSpawn + 4f));
            }

            while (ShopperQueue.Count > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(2f);

            var bullChecker = GameManager.Instance.DayNum - 1;

            if (bullChecker != 0 && bullChecker % GameManager.Instance.NumOfDaysBetweenBullEncounters == 0)
            {
                gameObject.AddComponent<BullEncounter>();
                yield break;
            }
            
            EndDay();
        }

        private Shopper LoadShopper()
        {
            var shopperObj = Instantiate(_shopperPrefab, _shopperSpawnCanvas, false);
            var img = shopperObj.GetComponent<Image>();
            img.sprite = _shopperSprites[Random.Range(0, _shopperSprites.Length)];
            img.SetNativeSize();
            
            var shopper = shopperObj.GetComponent<Shopper>();
            shopper.Controller = this;
            shopper.Mover = Mover;
            _remainingCustomers--;
            return shopper;
        }

        public bool RequestShopEntry()
        {
            if (ShopperQueue.Count > 3)
            {
                return false;
            }
            if (_doorOpenCoroutine != null) StopCoroutine(_doorOpenCoroutine);
            
            _doorOpenCoroutine = OpenDoor();

            StartCoroutine(_doorOpenCoroutine);

            return true;
        }

        public IEnumerator OpenDoor()
        {
            var isOpen = _doorAnimator.GetBool(ShouldOpen);
            
            if (!isOpen)
            {
                _doorAnimator.SetBool(ShouldOpen, true);
                
                yield return new WaitForSeconds(0.6f);
                
                yield return new WaitUntil(() => AnimatorIsPlaying() == false);
            }

            IsDoorOpen = _doorAnimator.GetBool(ShouldOpen);
        }
        
        public IEnumerator CloseDoor()
        {
            var isOpen = _doorAnimator.GetBool(ShouldOpen);
            
            if (isOpen)
            {
                _doorAnimator.SetBool(ShouldOpen, false);
                
                yield return new WaitForSeconds(0.6f);
                
                yield return new WaitUntil(() => AnimatorIsPlaying() == false);
            }

            IsDoorOpen = _doorAnimator.GetBool(ShouldOpen);
        }

        public bool CanJoinQueue()
        {
            return ShopperQueue.Count <= 3;
        }

        public void Remove(Shopper shopper)
        {
            ShopperQueue.Remove(shopper);
            StartCoroutine(MoveQueueAlong());
        }

        private IEnumerator MoveQueueAlong()
        {
            for (int i = 0; i < ShopperQueue.Count; i++)
            {
                ShopperQueue[i].MoveAlong(i);
                yield return new WaitForSeconds(0.3f);
            }
        }
        
        public bool RequestStock(StockType stock, int quantityToRequest)
        {
            if (!GameManager.Instance.AvailableStock.TryGetValue(stock, out var quantity)) return false;
        
            if (quantity < quantityToRequest) return false;
            
            GameManager.Instance.AvailableStock[stock] -= quantityToRequest;
            var earnings = (int)stock * quantityToRequest;
            GameManager.Instance.Cash += earnings;
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemSold, null);
            GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperServed, null);
        
            DayStats.CashEarned += earnings;
            DayStats.ShoppersServed++;
            return true;
        }
        
        public void EndDay()
        {
            GameManager.Instance.DayStats = DayStats;
            SceneFader.Instance.FadeToScene("Night");
        }

        private bool AnimatorIsPlaying(){
            return _doorAnimator.GetCurrentAnimatorStateInfo(0).length >
                   _doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }
}
