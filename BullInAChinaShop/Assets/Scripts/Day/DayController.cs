using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class DayController : MonoBehaviour
    {
        private const int MAX_NUM_OF_SPAWNED_SHOPPERS = 5;
        public List<Shopper> ShopperQueue { get; set; } = new List<Shopper>();

        public DayStats DayStats = new DayStats();
        
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

        private int _remainingCustomers;

        private void Start()
        {
            SubscribeListeners();
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
        
        public void RequestShopEntry(Dictionary<string, object> message)
        {
            var shopper = ShopperUtils.GetShopperFromMessage(message);
            if (ShopperQueue.Count > 3)
            {
                // shopper.OnRejectedEntry();
                GameEventsManager.Instance.TriggerEvent(GameEvent.ShopperDeniedEntry,
                    new Dictionary<string, object> {{"shopper", shopper}});
                return;
            }
            // Trigger animation for door opening
            // On complete:
            ShopperQueue.Add(shopper);
            GameEventsManager.Instance.TriggerEvent(GameEvent.DoorOpened,
                new Dictionary<string, object> {{"shopper", shopper}});
        }
        
        public void RequestShopEntry(BullEncounter bullEncounter)
        {
            // Trigger animation for door opening
            // On complete:
            bullEncounter.PlayBullEncounter(GameManager.Instance.DayNum);
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

        private IEnumerator StartDay()
        {
            while (_remainingCustomers > 0)
            {
                var shoppers = GameObject.FindObjectsOfType<Shopper>();
                LoadShopper();
                // if (shoppers.Length < 5)
                // {
                //     LoadShopper();
                // }
                yield return new WaitForSeconds(Random.Range(GameManager.Instance.MinTimeBetweenSpawn, GameManager.Instance.MinTimeBetweenSpawn + 0.2f));
                // yield return new WaitForSeconds(Random.Range(GameManager.Instance.MinTimeBetweenSpawn, GameManager.Instance.MinTimeBetweenSpawn + 4f));
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

        public void EndDay()
        {
            GameManager.Instance.DayStats = DayStats;
            SceneFader.Instance.FadeToScene("Night");
        }

        private void LoadShopper()
        {
            var shopperObj = Instantiate(_shopperPrefab, _shopperSpawnCanvas, false);
            var img = shopperObj.GetComponent<Image>();
            img.sprite = _shopperSprites[Random.Range(0, _shopperSprites.Length)];
            img.SetNativeSize();
            
            var shopper = shopperObj.GetComponent<Shopper>();
            shopper.Controller = this;
            _remainingCustomers--;
            shopper.WalkToDoor();
        }

        private void OnShopperQueued(Dictionary<string, object> message)
        {
            var shopper = ShopperUtils.GetShopperFromMessage(message);
            
            if (ShopperQueue.IndexOf(shopper) == 0)
            {
                StartCoroutine(shopper.Think());
                return;
            }

            StartCoroutine(shopper.Idle());
        }

        public void OnShopperExit(Dictionary<string, object> message)
        {
            var shopper = ShopperUtils.GetShopperFromMessage(message);
            var index = ShopperQueue.IndexOf(shopper);
            if (index == -1) return;
            
            ShopperQueue.Remove(shopper);
            StopCoroutine(nameof(MoveQueueAlong));
            StartCoroutine(MoveQueueAlong(index));
        }

        private IEnumerator MoveQueueAlong(int index)
        {
            if (index > ShopperQueue.Count - 1) yield break;
            for (var i = index; i < ShopperQueue.Count; i++)
            {
                if (ShopperQueue[i].IsLeaving) continue;
                ShopperQueue[i].MoveAlong(i);
                yield return new WaitForSeconds(0.3f);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeListeners();
        }

        private void UnsubscribeListeners()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.ShopperRequestingEntry, RequestShopEntry);
            GameEventsManager.Instance.RemoveListener(GameEvent.ShopperLeaving, OnShopperExit);
            GameEventsManager.Instance.RemoveListener(GameEvent.ShopperQueued, OnShopperQueued);
        }

        private void SubscribeListeners()
        {
            GameEventsManager.Instance.AddListener(GameEvent.ShopperRequestingEntry, RequestShopEntry);
            GameEventsManager.Instance.AddListener(GameEvent.ShopperLeaving, OnShopperExit);
            GameEventsManager.Instance.AddListener(GameEvent.ShopperQueued, OnShopperQueued);
        }
    }
}
