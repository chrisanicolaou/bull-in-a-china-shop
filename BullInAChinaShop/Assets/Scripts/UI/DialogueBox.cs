using System;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.UI
{
    public class DialogueBox : Singleton<DialogueBox>
    {
        [SerializeField]
        private GameObject _dialogueBox;

        [SerializeField]
        private TextMeshProUGUI _dialogueHeader;

        [SerializeField]
        private TextMeshProUGUI _dialogueBody;

        private string _headerText;

        private string _bodyText;

        private bool _isShowing;

        private bool _isComplete;

        private bool _shouldStayOnScreen;

        private float _textDisplayDuration;

        private TweenerCore<string, string, StringOptions> _currentTween;

        private Action _callback;

        public void Start()
        {
            _dialogueBox ??= GameObject.FindWithTag("DialogueBox");
            _dialogueHeader ??= GameObject.FindWithTag("DialogueHeaderText").GetComponent<TextMeshProUGUI>();
            _dialogueBody ??= GameObject.FindWithTag("DialogueBodyText").GetComponent<TextMeshProUGUI>();
        }

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            
            if (_isComplete)
            {
                if (!_shouldStayOnScreen)
                {
                    _dialogueBox.transform.DOScale(new Vector3(0f, 0f, 0f), 0.1f)
                        .OnComplete(() =>
                        {
                            _callback?.Invoke();
                            Clean();
                        });
                }
                else
                {
                    _callback?.Invoke();
                    Clean();
                }
            }
                
            if (_isShowing)
            {
                _currentTween.Kill();
                _dialogueBody.text = _bodyText;
                _isComplete = true;
            }
        }

        public DialogueBox SetHeader(string header)
        {
            _headerText = header;
            return this;
        }

        public DialogueBox SetBody(string body)
        {
            _bodyText = body;
            _textDisplayDuration = Math.Min(50f / body.Length, 2f);
            return this;
        }

        public DialogueBox OnComplete(Action callback)
        {
            _callback += callback;
            return this;
        }

        public DialogueBox StayOnScreen()
        {
            _shouldStayOnScreen = true;
            return this;
        }

        public DialogueBox Show()
        {
            _dialogueHeader.text = _headerText;
            _dialogueBox.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f)
                .OnComplete(() =>
                {
                    _isShowing = true;
                    _currentTween = _dialogueBody.DOText(_bodyText, _textDisplayDuration)
                        .OnComplete(() => _isComplete = true);
                });
            return this;
        }
        
        private void Clean()
        {
            _headerText = default;
            _bodyText = default;
            _isShowing = default;
            _isComplete = default;
            _shouldStayOnScreen = default;
            _textDisplayDuration = default;
            _currentTween = null;
            _callback = null;
        }
        
    }
}
