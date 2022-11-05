using System;
using System.Collections;
using CharaGaming.BullInAChinaShop.Singletons;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class BullEncounter : MonoBehaviour
    {
        public DayController Controller { get; set; }

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _startScale;

        [SerializeField]
        private float _walkSpeed = 1f;

        [SerializeField]
        private Vector3 _startPosition;

        [SerializeField]
        private Vector3 _doorPosition;

        [SerializeField]
        private Vector3 _centerOfTillPosition;

        [SerializeField]
        private GameObject _bullPrefab;
        
        private Transform _shopperSpawnCanvas;

        private RectTransform _bullRect;

        private GameObject _bull;
        
        private void Start()
        {
            _shopperSpawnCanvas = GameObject.FindWithTag("SpawnCanvas").GetComponent<Transform>();
            var _bull = CreateBull();
            WalkToDoor();
        }

        private GameObject CreateBull()
        {
            var bullObj = Instantiate(_bullPrefab, _shopperSpawnCanvas, false);
            _bullRect = bullObj.GetComponent<RectTransform>();
            _bullRect.anchoredPosition = _startPosition;
            _bullRect.localScale = new Vector3(_startScale, _startScale, _startScale);
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
            var walkingToDesk = WalkToDesk();
            
            yield return new WaitUntil(() => !walkingToDesk.active);
            
            PrepareDialogue("Wow - I love what you've done with the place!");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("I tell you somethin', this be the best china shop I ever did see in my life.");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("Say - you will do well here for sure.");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);

            var walkingBackToDoor = WalkBackToDoor();
            
            yield return new WaitUntil(() => !walkingBackToDoor.active);
            
            PrepareDialogue("Oh, that reminds me. Remember that teeny, tiny, <color=\"red\">15000</color> loan?");

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);
            
            PrepareDialogue("Well, I'll be back in 3 days to collect.", false);

            yield return new WaitForSeconds(0.3f);
            yield return new WaitUntil(() => DialogueBox.Instance.CurrentTween == null);

            _bullRect.SetSiblingIndex(1);
            _bullRect.DOAnchorPos(_startPosition, _walkSpeed).SetEase(Ease.Linear);
        }

        public void WalkToDoor()
        {
            _bullRect.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);
            _bullRect.DOAnchorPos(_doorPosition, _walkSpeed).SetEase(Ease.Linear)
                .OnComplete(() => { Controller.RequestShopEntry(this); });
        }
        
        public TweenerCore<Vector2, Vector2, VectorOptions> WalkBackToDoor()
        {
            _bullRect.DOScale(_startScale, _walkSpeed).SetEase(Ease.Linear);
            return _bullRect.DOAnchorPos(_doorPosition, _walkSpeed).SetEase(Ease.Linear);
        }

        public TweenerCore<Vector2, Vector2, VectorOptions> WalkToDesk()
        {
            var siblingIndex = _bullRect.parent.childCount - 3;
            _bullRect.SetSiblingIndex(siblingIndex);
            _bullRect.DOScale(1f, _walkSpeed).SetEase(Ease.Linear);
            return _bullRect.DOAnchorPos(_centerOfTillPosition, _walkSpeed).SetEase(Ease.Linear);
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