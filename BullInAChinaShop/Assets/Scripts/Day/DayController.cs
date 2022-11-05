using System.Collections;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class DayController : MonoBehaviour
    {
        public Queue<Shopper> ShopperQueue { get; set; } = new Queue<Shopper>();

        public DayStats DayStats = new DayStats();
        
        [SerializeField]
        private Button _startDayButton;

        [SerializeField]
        private GameObject _shopperPrefab;

        [SerializeField]
        private Sprite[] _shopperSprites;

        [SerializeField]
        private Transform _shopperSpawnCanvas;

        private int _remainingCustomers;

        private void Start()
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
                LoadShopper();
                yield return new WaitForSeconds(Random.Range(GameManager.Instance.MinTimeBetweenSpawn, GameManager.Instance.MinTimeBetweenSpawn + 4f));
            }

            while (ShopperQueue.Count > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(2f);
            EndDay();
        }

        private void EndDay()
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

        public void RequestShopEntry(Shopper shopper)
        {
            // Trigger animation for door opening
            // On complete:
            ShopperQueue.Enqueue(shopper);
            shopper.WalkToTill(ShopperQueue.Count);
        }

        public bool RequestStock(StockType stock, int quantityToRequest)
        {
            if (!GameManager.Instance.AvailableStock.TryGetValue(stock, out var quantity)) return false;

            if (quantity < quantityToRequest) return false;
            
            GameManager.Instance.AvailableStock[stock] -= quantityToRequest;
            var profit = (int)stock * quantityToRequest;
            GameManager.Instance.Cash += profit;
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemSold, null);

            DayStats.CashEarned += profit;
            DayStats.ShoppersServed++;
            return true;
        }
    }
}
