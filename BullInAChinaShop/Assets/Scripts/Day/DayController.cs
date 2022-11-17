using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class DayController : MonoBehaviour
    {
        public List<Shopper> ShopperQueue { get; set; } = new List<Shopper>();

        public DayStats DayStats { get; } = new DayStats();

        [SerializeField]
        private GameObject _stockMenuTutorialText;
        
        [SerializeField]
        private GameObject _startDayTutorialText;
        
        [SerializeField]
        private Button _startDayButton;
        
        [SerializeField]
        private Button _purchaseMenuButton;
        
        [SerializeField]
        private GameObject _purchaseMenuLight;

        [SerializeField]
        private GameObject _purchaseMenu;

        [SerializeField]
        private GameObject _bullEncounterPrefab;

        [SerializeField]
        private GameObject[] _shoppers;

        [SerializeField]
        private Transform _shopperSpawnCanvas;

        [SerializeField]
        private Animator _doorAnimator;
        
        [field: SerializeField]
        public CharacterMover Mover { get; set; }

        private int _remainingCustomers;

        public bool IsDoorOpen { get; set; }

        public IEnumerator OpenDoorCoroutine { get; set; }
        
        public IEnumerator CloseDoorCoroutine { get; set; }

        private static readonly int ShouldOpen = Animator.StringToHash("shouldOpen");

        private void Start()
        {
            OpenDoorCoroutine = OpenDoor();
            CloseDoorCoroutine = CloseDoor();
            _remainingCustomers = Random.Range(GameManager.Instance.MinCustomers, GameManager.Instance.MinCustomers + 4);
            
            if (GameManager.Instance.BullEncounterDays.Contains(GameManager.Instance.DayNum))
            {
                var bullEncounter = Instantiate(_bullEncounterPrefab).GetComponent<BullEncounter>();
                bullEncounter.Controller = this;
                bullEncounter.Mover = Mover;
                bullEncounter.PlayBullEncounter(GameManager.Instance.DayNum);
                return;
            }
            StartDay();
        }

        public void StartDay()
        {
            if (GameManager.Instance.DayNum == 1)
            {
                TogglePurchaseMenuButton();
                _stockMenuTutorialText.SetActive(true);
                
                GameEventsManager.Instance.AddListener(GameEvent.CashChanged, ToggleTutorialText);
                return;
            }
            ToggleStartDayButton();
            TogglePurchaseMenuButton();
        }

        private void ToggleTutorialText(Dictionary<string, object> message)
        {
            _stockMenuTutorialText.SetActive(false);
            _startDayTutorialText.SetActive(true);
            ToggleStartDayButton();
            GameEventsManager.Instance.RemoveListener(GameEvent.CashChanged, ToggleTutorialText);
        }

        public void ToggleStartDayButton(bool toggle = true)
        {
            if (toggle)
            {
                _startDayButton.enabled = true;
                _startDayButton.onClick.AddListener(() =>
                {
                    StartCoroutine(nameof(StartDayCoroutine));
                    if (_startDayTutorialText.activeSelf) _startDayTutorialText.SetActive(false);
                    Destroy(_startDayButton.gameObject);
                });
            }
            else
            {
                _startDayButton.onClick.RemoveAllListeners();
                _startDayButton.enabled = false;
            }
        }

        public void TogglePurchaseMenuButton(bool toggle = true)
        {
            if (toggle)
            {
                _purchaseMenuButton.enabled = true;
                _purchaseMenuButton.onClick.AddListener(() =>
                {
                    _purchaseMenu.SetActive(true);
                });
                _purchaseMenuLight.SetActive(true);
            }
            else
            {
                _purchaseMenuButton.onClick.RemoveAllListeners();
                _purchaseMenuButton.enabled = false;
                _purchaseMenuLight.SetActive(false);
            }
        }

        private IEnumerator StartDayCoroutine()
        {
            ToggleStartDayButton(false);
            TogglePurchaseMenuButton(false);
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

            yield return new WaitForSeconds(4f);
            
            EndDay();
        }

        private Shopper LoadShopper()
        {
            var shopperObj = Instantiate(_shoppers[Random.Range(0, _shoppers.Length)], _shopperSpawnCanvas, false);
            var img = shopperObj.GetComponent<Image>();
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
            if (OpenDoorCoroutine != null) StopCoroutine(OpenDoorCoroutine);
            
            OpenDoorCoroutine = OpenDoor();

            StartCoroutine(OpenDoorCoroutine);

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
            OpenDoorCoroutine = OpenDoor();
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
            CloseDoorCoroutine = CloseDoor();
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
        
        public bool RequestStock(BaseStock requestedStock, int quantityToRequest)
        {
            if (requestedStock.AvailableQuantity < quantityToRequest) return false;
            
            requestedStock.AvailableQuantity -= quantityToRequest;
            var earnings = requestedStock.SellValue * quantityToRequest;
            GameManager.Instance.Cash += earnings;
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemSold, new Dictionary<string, object> { { "stock", requestedStock }, { "quantity", quantityToRequest }});
            GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperServed, null);
        
            DayStats.CashEarned += earnings;
            DayStats.ShoppersServed++;
            return true;
        }

        private void EndDay()
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
