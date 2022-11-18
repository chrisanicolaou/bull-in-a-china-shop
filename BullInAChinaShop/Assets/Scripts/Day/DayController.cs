using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.UI.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        private Image _speedUpImg;
        
        [SerializeField]
        private AudioClip _preDayMusic;
        
        [SerializeField]
        private AudioClip _dayMusic;
        
        [SerializeField]
        private AudioClip _dayEndMusic;

        [SerializeField]
        private AudioSource _musicController;

        [SerializeField]
        private AudioSource _doorSfxController;

        [SerializeField]
        private GameObject _stockMenuTutorialText;

        [SerializeField]
        private GameObject _startDayTutorialText;

        [SerializeField]
        private Button _startDayButton;
        
        [SerializeField]
        private Animator _startDayButtonAnim;

        [SerializeField]
        private GameObject _startDayLight;

        [SerializeField]
        private Sprite[] _currentSpeedSprite;

        [SerializeField]
        private float[] _speedSettings;

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

        private int _speedIndex;

        private IEnumerator _moveAlongCoroutine;

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
            ChangeMusic(_preDayMusic);
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

        public void ReassignTill(GameObject tillObj)
        {
            _purchaseMenuButton = tillObj.GetComponent<Button>();
            _purchaseMenuLight = tillObj.FindComponentInChildWithTag<Light2D>("TillLight").gameObject;
            TogglePurchaseMenuButton();
        }

        private void ToggleSpeed()
        {
            if (++_speedIndex >= _speedSettings.Length) _speedIndex = 0;
            Time.timeScale = _speedSettings[_speedIndex];
            _speedUpImg.sprite = _currentSpeedSprite[_speedIndex];
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
                _startDayLight.SetActive(true);
                _startDayButton.onClick.AddListener(() => StartCoroutine(OnStartDayButtonPress()));
            }
            else
            {
                _startDayButton.onClick.RemoveAllListeners();
                _startDayButton.enabled = false;
            }
        }

        private IEnumerator OnStartDayButtonPress()
        {
            _startDayButton.enabled = false;
            _startDayLight.SetActive(false);
            _startDayButtonAnim.SetBool("shouldFlip", true);

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => AnimatorIsPlaying(_startDayButtonAnim) == false);

            _startDayButtonAnim.SetBool("shouldFlip", false);
            yield return null;

            _startDayButtonAnim.enabled = false;
            _speedUpImg.sprite = _currentSpeedSprite[0];

            _startDayButton.enabled = true;
            _startDayButton.onClick.RemoveAllListeners();
            _startDayButton.onClick.AddListener(ToggleSpeed);
            
            StartCoroutine(nameof(StartDayCoroutine));
            if (_startDayTutorialText.activeSelf) _startDayTutorialText.SetActive(false);
            ChangeMusic(_dayMusic);
        }

        public void TogglePurchaseMenuButton(bool toggle = true)
        {
            if (toggle)
            {
                _purchaseMenuButton.enabled = true;
                _purchaseMenuButton.onClick.AddListener(() => { _purchaseMenu.SetActive(true); });
                _purchaseMenuLight.SetActive(true);
                
                ChangeMusic(_preDayMusic);
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
            TogglePurchaseMenuButton(false);
            while (_remainingCustomers > 0)
            {
                var shopper = LoadShopper();
                StartCoroutine(shopper.ApproachShop());
                yield return new WaitForSeconds(Random.Range(GameManager.Instance.MinTimeBetweenSpawn, GameManager.Instance.MinTimeBetweenSpawn + 4f));
            }

            while (_shopperSpawnCanvas.childCount > 6)
            {
                yield return new WaitForSeconds(1f);
            }
            
            ChangeMusic(_dayEndMusic, false);

            yield return new WaitForSeconds(2f);

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

                yield return new WaitForSeconds(0.3f);

                _doorSfxController.Play();

                yield return new WaitForSeconds(0.3f);

                yield return new WaitUntil(() => AnimatorIsPlaying(_doorAnimator) == false);
                
                IsDoorOpen = _doorAnimator.GetBool(ShouldOpen);
            }
            
            OpenDoorCoroutine = OpenDoor();
        }

        public IEnumerator CloseDoor()
        {
            var isOpen = _doorAnimator.GetBool(ShouldOpen);

            if (isOpen)
            {
                IsDoorOpen = false;
                _doorAnimator.SetBool(ShouldOpen, false);

                yield return new WaitForSeconds(0.6f);

                yield return new WaitUntil(() => AnimatorIsPlaying(_doorAnimator) == false);
            }

            CloseDoorCoroutine = CloseDoor();
        }

        public bool CanJoinQueue()
        {
            return ShopperQueue.Count <= 3;
        }

        public void Remove(Shopper shopper)
        {
            ShopperQueue.Remove(shopper);
            if (_moveAlongCoroutine != null) StopCoroutine(_moveAlongCoroutine);
            _moveAlongCoroutine = MoveQueueAlong();
            StartCoroutine(_moveAlongCoroutine);
        }

        private IEnumerator MoveQueueAlong()
        {
            for (int i = 0; i < ShopperQueue.Count; i++)
            {
                ShopperQueue[i].MoveAlong(i);
                yield return new WaitForSeconds(0.3f);
            }
            _moveAlongCoroutine = null;
        }

        public bool RequestStock(BaseStock requestedStock, int quantityToRequest)
        {
            if (requestedStock.AvailableQuantity < quantityToRequest) return false;

            requestedStock.AvailableQuantity -= quantityToRequest;
            var earnings = requestedStock.SellValue * quantityToRequest;
            GameManager.Instance.Cash += earnings;
            GameEventsManager.Instance.TriggerEvent(GameEvent.ItemSold, new Dictionary<string, object> { { "stock", requestedStock }, { "quantity", quantityToRequest } });
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

        private bool AnimatorIsPlaying(Animator animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length >
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        private void ChangeMusic(AudioClip targetClip, bool loop = true)
        {
            _musicController.Pause();
            _musicController.clip = targetClip;
            _musicController.loop = loop;
            _musicController.Play();
        }
    }
}