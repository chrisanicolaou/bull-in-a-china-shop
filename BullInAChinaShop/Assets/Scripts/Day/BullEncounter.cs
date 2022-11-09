using System;
using System.Collections;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
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
            if (dayNum == 1)
            {
                StartCoroutine(nameof(FirstBullEncounter));
                return;
            }

        }

        private IEnumerator FirstBullEncounter()
        {
            yield return StartCoroutine(ApproachShop());

            yield return StartCoroutine(WalkInsideDoor());

            yield return StartCoroutine(WalkToDesk());
            
            PrepareDialogue("Wow - I love what you've done with the place!");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("I tell you somethin', this be the best china shop I ever did see in my life.");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("Say - you will do well here for sure.", false);

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);

            yield return StartCoroutine(WalkInsideDoor());
            
            PrepareDialogue("Oh, that reminds me. Remember that teeny, tiny, <color=\"red\">15000</color> loan?");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("Well, I'll be back in 3 days to collect.", false);

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);

            yield return StartCoroutine(ExitDoor());
            var seq = Mover.MoveTo(_bullRect, _startPosition.PosAndScale.pos, _startPosition.PosAndScale.scale, true);

            yield return seq.WaitForCompletion();
            
            OnEncounterFinish();
        }

        private void OnEncounterFinish()
        {
            Controller.ToggleStartAndUpgradeButtons();
            Destroy(_bull);
            Destroy(gameObject);
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

        private void PrepareDialogue(string body, bool stayOnScreen = true)
        {
            DialogueBox.Instance.SetHeader("Mr. Bull")
                .SetBody(body)
                .StayOnScreen(stayOnScreen)
                .Show();
        }
    }
}