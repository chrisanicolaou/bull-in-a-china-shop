using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Utils;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class BullEncounter : MonoBehaviour
    {
        // Be nice to do this with some sort of severity flag to change how pissed off his prompts are!
        //
        // private readonly List<string> _randomAngryPrompts = new List<string>()
        // {
        //     "You know what I'm here for...",
        //     "I'm getting real tired of this.",
        //     "You are <color=\"red\">REALLY starting to test my patience.</b> "
        // }
        public DayController Controller { get; set; }

        public CharacterMover Mover { get; set; }

        public List<AudioSource> BreakSfxChannels { get; set; }

        [SerializeField]
        private AudioClip[] _breakSfxClips;

        [SerializeField]
        private ShopPosition _startPosition;

        [FormerlySerializedAs("_doorPosition")]
        [SerializeField]
        private ShopPosition _outsideDoorPosition;

        [SerializeField]
        private ShopPosition _insideDoorPosition;

        [SerializeField]
        private ShopPosition _centerOfTillPosition;

        [SerializeField]
        private GameObject _bullPrefab;

        [SerializeField]
        private int _numberOfBounces;

        [SerializeField]
        [Range(0.01f, 0.3f)]
        private float _percentCashToLosePerBounce = 0.02f;

        [SerializeField]
        [Range(0.01f, 0.3f)]
        private float _percentStockToLosePerBounce = 0.02f;

        [SerializeField]
        [Range(0.01f, 1f)]
        private float _screenShakeMagnitude = 1f;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _screenShakeDuration = 1f;

        private Transform _shopperSpawnCanvas;

        private RectTransform _bullRect;

        private GameObject _bull;

        private Animator _animator;

        private Camera _cam;

        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int IsAnnoyed = Animator.StringToHash("isBlowingSmoke");
        private static readonly int IsWalkingForward = Animator.StringToHash("isWalkingForward");
        private static readonly int IsWalkingAway = Animator.StringToHash("isWalkingAway");
        private static readonly int IsWalkingSide = Animator.StringToHash("isWalkingSide");
        private static readonly int IsTornado = Animator.StringToHash("isTornado");
        private static readonly int IsBlowingSmoke = Animator.StringToHash("isBlowingSmoke");

        private List<int> _animatorIds;

        private void Awake()
        {
            _shopperSpawnCanvas = GameObject.FindWithTag("SpawnCanvas").GetComponent<Transform>();
            _bull = CreateBull();
            _animatorIds = new List<int> { IsIdle, IsAnnoyed, IsWalkingAway, IsWalkingForward, IsWalkingSide, IsTornado, IsBlowingSmoke };
        }

        private GameObject CreateBull()
        {
            var bullObj = Instantiate(_bullPrefab, _shopperSpawnCanvas, false);
            _bullRect = bullObj.GetComponent<RectTransform>();
            _bullRect.anchoredPosition = _startPosition.PosAndScale.pos;
            _bullRect.localScale = new Vector3(_startPosition.PosAndScale.scale, _startPosition.PosAndScale.scale, _startPosition.PosAndScale.scale);
            _bullRect.SetSiblingIndex(1);
            _animator = bullObj.GetComponent<Animator>();

            return bullObj;
        }

        public void PlayBullEncounter(int dayNum)
        {
            var bullEncounterIndex = GameManager.Instance.BullEncounterDays.IndexOf(dayNum);

            if (bullEncounterIndex == -1)
            {
                Debug.LogError("Bull encounter does not exist in BullEncounterDays!");
                return;
            }

            if (bullEncounterIndex == GameManager.Instance.BullEncounterDays.Count - 1)
            {
                StartCoroutine(nameof(LastBullEncounter));
                return;
            }

            switch (bullEncounterIndex)
            {
                case 0:
                    StartCoroutine(nameof(FirstBullEncounter));
                    break;
                case 1:
                    StartCoroutine(nameof(SecondBullEncounter));
                    break;
                default:
                    StartCoroutine(nameof(AngryBullEncounter));
                    break;
            }
        }

        private IEnumerator FirstBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            Animate(IsWalkingForward);

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue("Wow - I love what you've done with the place!"));

            yield return StartCoroutine(PrepareDialogue("I tell you somethin', this be the best china shop I ever did see in my life."));

            yield return StartCoroutine(PrepareDialogue("Say - you will do well here for sure.", false));

            Animate(IsWalkingAway);

            yield return StartCoroutine(WalkInsideDoor());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue($"Oh, that reminds me. Remember that teeny, tiny, {$"$ {GameManager.Instance.LoanAmount.KiloFormat()}".ToTMProColor(Color.red)} loan?"));

            yield return StartCoroutine(PrepareDialogue($"Well, I'll be back in {GameManager.Instance.DaysUntilNextBullEncounter} days to collect.", false));

            yield return StartCoroutine(ExitDoor());

            Animate(IsWalkingSide);

            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();

            OnEncounterFinish();
        }

        private IEnumerator SecondBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            Animate(IsWalkingForward);

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue("Well, well. The day has come."));

            yield return StartCoroutine(PrepareDialogue("Do you have my money?"));

            yield return StartCoroutine(PrepareDialogue("..."));

            yield return StartCoroutine(PrepareDialogue("I see."));

            yield return StartCoroutine(PrepareDialogue("So it has come to this."));

            Animate(IsBlowingSmoke);

            yield return StartCoroutine(PrepareDialogue($"Mr. Shopkeeper, you are now {"LATE.".ToTMProColor(Color.red)}"));

            yield return StartCoroutine(PrepareDialogue($"You have {(GameManager.Instance.TotalNumOfDays - GameManager.Instance.DayNum).ToString().ToTMProColor(Color.red)} days to get me my money."));

            yield return StartCoroutine(PrepareDialogue("If you don't have it by then I'll..."));

            yield return StartCoroutine(PrepareDialogue("...I'll get reaaal clumsy.", false));

            Animate(IsWalkingAway);

            yield return StartCoroutine(WalkInsideDoor());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue($"I'll be back in {GameManager.Instance.DaysUntilNextBullEncounter.ToString().ToTMProColor(Color.red)} days..."));

            yield return StartCoroutine(PrepareDialogue($"You know, to...check up on you.", false));

            yield return StartCoroutine(ExitDoor());

            Animate(IsWalkingSide);

            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();

            OnEncounterFinish();
        }

        private IEnumerator AngryBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            Animate(IsWalkingForward);

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue("You know what I'm here for."));

            yield return StartCoroutine(PrepareDialogue("Do you have my money?"));

            Animate(IsBlowingSmoke);

            yield return StartCoroutine(PrepareDialogue("That's it - <color=\"red\">YOU'LL PAY</color> for this!", false));

            yield return StartCoroutine(Tornado());

            yield return StartCoroutine(PrepareDialogue("I warned you this would happen.", false));

            Animate(IsWalkingAway);

            yield return StartCoroutine(WalkInsideDoor());

            Animate(IsIdle);

            yield return StartCoroutine(PrepareDialogue($"I'll be back again in {GameManager.Instance.DaysUntilNextBullEncounter.ToString().ToTMProColor(Color.red)} days."));

            yield return StartCoroutine(PrepareDialogue($"You better have my money.", false));

            yield return StartCoroutine(ExitDoor());

            Animate(IsWalkingSide);

            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();

            OnEncounterFinish();
        }

        private IEnumerator LastBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            Animate(IsWalkingForward);

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            Animate(IsBlowingSmoke);

            yield return StartCoroutine(PrepareDialogue("This has gone on long enough."));

            yield return StartCoroutine(PrepareDialogue("Your time has come."));

            yield return StartCoroutine(PrepareDialogue("I should have NEVER believed in you!"));

            StartCoroutine(Tornado());

            yield return new WaitForSeconds(1f);

            var volume = GameObject.FindWithTag("GlobalVolume").GetComponent<Volume>();

            volume.profile.TryGet(out Bloom bloom);

            DOTween.To(() => bloom.threshold.value, (x) => bloom.threshold.value = x, 0f, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    DOTween.To(() => bloom.intensity.value, (x) => bloom.intensity.value = x, 15f, 5f).SetEase(Ease.OutQuad)
                        .OnComplete(() => { SceneManager.LoadScene("Defeat"); });
                });
        }

        private void OnEncounterFinish()
        {
            Controller.StartDay();
            Destroy(_bull);
            Destroy(this);
        }

        public IEnumerator ApproachShop()
        {
            Animate(IsWalkingSide);
            var seq = Mover.MoveTo(_bullRect, _outsideDoorPosition.PosAndScale.pos, _outsideDoorPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();

            Animate(IsIdle);

            Controller.RequestShopEntry();

            yield return new WaitUntil(() => Controller.IsDoorOpen);
        }

        public IEnumerator WalkInsideDoor()
        {
            var seq = Mover.MoveTo(_bullRect, _insideDoorPosition.PosAndScale.pos, _insideDoorPosition.PosAndScale.scale, true);

            yield return new WaitForSeconds(0.05f);

            _bullRect.SetSiblingIndex(_bullRect.parent.childCount - 3);

            yield return seq.WaitForCompletion();
        }

        public IEnumerator ExitDoor()
        {
            Controller.StartCoroutine(Controller.OpenDoorCoroutine);

            yield return new WaitUntil(() => Controller.IsDoorOpen);

            Animate(IsWalkingAway);

            var seq = Mover.MoveTo(_bullRect, _outsideDoorPosition.PosAndScale.pos, _outsideDoorPosition.PosAndScale.scale, true);

            yield return new WaitForSeconds(0.05f);

            _bullRect.SetSiblingIndex(1);

            yield return seq.WaitForCompletion();

            Controller.StartCoroutine(Controller.CloseDoorCoroutine);
        }

        public IEnumerator WalkToDesk()
        {
            var siblingIndex = _bullRect.parent.childCount - 3;
            _bullRect.SetSiblingIndex(siblingIndex);

            var seq = Mover.MoveTo(_bullRect, _centerOfTillPosition.PosAndScale.pos, _centerOfTillPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();
        }

        private IEnumerator PrepareDialogue(string body, bool stayOnScreen = true)
        {
            DialogueBox.Instance.SetHeader("Mr. Bull")
                .SetBody(body)
                .StayOnScreen(stayOnScreen)
                .Show();

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
        }

        private IEnumerator Tornado(bool endless = false)
        {
            Animate(IsTornado);

            List<ShopLocation> locations = new List<ShopLocation> { ShopLocation.LoiterOne, ShopLocation.LoiterTwo, ShopLocation.LoiterThree };

            for (int i = 0; endless ? i < 34 : i < _numberOfBounces; i++)
            {
                var location = locations[i % locations.Count];

                var seq = Mover.MoveTo(_bullRect, location, durationMultiplier: 0.25f);

                yield return seq.WaitForCompletion();
                ShakeScreen(_screenShakeDuration, _screenShakeMagnitude);
                if (!endless)
                {
                    DestroyStockAndCash();
                }
            }

            _cam.transform.DOScale(new Vector3(1, 1, 1), _screenShakeDuration);

            yield return StartCoroutine(WalkToDesk());

            Animate(IsBlowingSmoke);
        }

        private void DestroyStockAndCash()
        {
            var sfxChannel = BreakSfxChannels.FirstOrDefault(c => !c.isPlaying);
            if (sfxChannel != null) sfxChannel.PlayOneShot(_breakSfxClips[Random.Range(0, _breakSfxClips.Length)]);
            var minCashLoss = Mathf.CeilToInt(GameManager.Instance.Cash * _percentCashToLosePerBounce);
            if (minCashLoss != 0)
            {
                var randomLoss = Mathf.CeilToInt(Random.Range(minCashLoss, minCashLoss + minCashLoss * 0.2f));
                GameManager.Instance.Cash -= randomLoss > GameManager.Instance.Cash ? minCashLoss : randomLoss;
            }

            var availableStock = GameManager.Instance.AvailableStock.Where(s => s.AvailableQuantity > 0).ToList();

            if (availableStock.Count > 0)
            {
                var stockToLose = availableStock[Random.Range(0, availableStock.Count)];
                var minQuantityToLose = Mathf.CeilToInt(stockToLose.AvailableQuantity * _percentStockToLosePerBounce);
                var randomQuantityToLose = Mathf.CeilToInt(Random.Range(minQuantityToLose, minQuantityToLose + minQuantityToLose * 0.2f));
                stockToLose.AvailableQuantity -= randomQuantityToLose > stockToLose.AvailableQuantity ? minQuantityToLose : randomQuantityToLose;
                GameEventsManager.Instance.TriggerEvent(GameEvent.StockDestroyed, new Dictionary<string, object> { { "item", stockToLose } });
            }
        }

        public void ShakeScreen(float duration, float magnitude)
        {
            _cam ??= Camera.main;
            _cam.backgroundColor = Color.black;
            _cam.transform.DOShakeScale(_screenShakeDuration, _screenShakeMagnitude);
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