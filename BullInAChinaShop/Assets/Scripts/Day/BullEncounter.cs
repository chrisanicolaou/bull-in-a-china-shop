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
using UnityEngine.Serialization;

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
        
        private Transform _shopperSpawnCanvas;

        private RectTransform _bullRect;

        private GameObject _bull;
        
        private void Awake()
        {
            _shopperSpawnCanvas = GameObject.FindWithTag("SpawnCanvas").GetComponent<Transform>();
            _bull = CreateBull();
        }

        private GameObject CreateBull()
        {
            var bullObj = Instantiate(_bullPrefab, _shopperSpawnCanvas, false);
            _bullRect = bullObj.GetComponent<RectTransform>();
            _bullRect.anchoredPosition = _startPosition.PosAndScale.pos;
            _bullRect.localScale = new Vector3(_startPosition.PosAndScale.scale, _startPosition.PosAndScale.scale, _startPosition.PosAndScale.scale);
            _bullRect.SetSiblingIndex(1);

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

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            yield return StartCoroutine(PrepareDialogue("Wow - I love what you've done with the place!"));
            
            yield return StartCoroutine(PrepareDialogue("I tell you somethin', this be the best china shop I ever did see in my life."));
            
            yield return StartCoroutine(PrepareDialogue("Say - you will do well here for sure.", false));

            yield return StartCoroutine(WalkInsideDoor());
            
            yield return StartCoroutine(PrepareDialogue($"Oh, that reminds me. Remember that teeny, tiny, {$"$ {GameManager.Instance.LoanAmount.KiloFormat()}".ToTMProColor(Color.red)} loan?"));
            
            yield return StartCoroutine(PrepareDialogue($"Well, I'll be back in {GameManager.Instance.DaysUntilNextBullEncounter} days to collect.", false));

            yield return StartCoroutine(ExitDoor());
            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();
            
            OnEncounterFinish();
        }
        
        private IEnumerator SecondBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());

            yield return StartCoroutine(PrepareDialogue("Well, well. The day has come."));
            
            yield return StartCoroutine(PrepareDialogue("Do you have my money?"));
            
            yield return StartCoroutine(PrepareDialogue("..."));
            
            yield return StartCoroutine(PrepareDialogue("I see."));
            
            yield return StartCoroutine(PrepareDialogue("So it has come to this."));
            
            // Some sort of animation
            
            yield return StartCoroutine(PrepareDialogue($"Mr. Shopkeeper, you are now {"LATE.".ToTMProColor(Color.red)}"));
            
            yield return StartCoroutine(PrepareDialogue($"You have {(GameManager.Instance.TotalNumOfDays - GameManager.Instance.DayNum).ToString().ToTMProColor(Color.red)} days to get me my money."));

            yield return StartCoroutine(PrepareDialogue("If you don't have it by then I'll..."));
            
            yield return StartCoroutine(PrepareDialogue("...I'll get reaaal clumsy.", false));
            
            yield return StartCoroutine(WalkInsideDoor());
            
            yield return StartCoroutine(PrepareDialogue($"I'll be back in {GameManager.Instance.DaysUntilNextBullEncounter} days..."));
            
            yield return StartCoroutine(PrepareDialogue($"You know, to...check up on you.", false));
            
            yield return StartCoroutine(ExitDoor());
            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();
            
            OnEncounterFinish();
        }

        private IEnumerator AngryBullEncounter()
        {
            throw new NotImplementedException();
        }

        private IEnumerator LastBullEncounter()
        {
            yield break;
        }

        private void OnEncounterFinish()
        {
            Controller.StartDay();
            Destroy(_bull);
            Destroy(this);
        }

        public IEnumerator ApproachShop()
        {
            var seq = Mover.MoveTo(_bullRect, _outsideDoorPosition.PosAndScale.pos, _outsideDoorPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();

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
    }
}