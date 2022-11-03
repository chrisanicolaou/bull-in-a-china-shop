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

        [SerializeField]
        private AudioSource _audioSource;

        private string _headerText;

        private string _bodyText;

        private bool _isShowing;

        private bool _isComplete;

        private bool _shouldStayOnScreen;

        private bool _cleanOnComplete = true;

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
                            if (_cleanOnComplete) Clean();
                        });
                }
                else
                {
                    _callback?.Invoke();
                    if (_cleanOnComplete) Clean(true);
                }
            } 
            else if (_isShowing)
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

        public DialogueBox OnComplete(params Action[] callbacks)
        {
            _callback = null;
            foreach (var callback in callbacks)
            {
                _callback += callback;
            }
            return this;
        }

        public DialogueBox StayOnScreen()
        {
            _shouldStayOnScreen = true;
            return this;
        }

        public DialogueBox AddAudio(AudioClip clip)
        {
            _audioSource.clip = clip;
            return this;
        }

        public DialogueBox DontCleanOnComplete()
        {
            _cleanOnComplete = false;
            return this;
        }

        public DialogueBox Show()
        {
            _dialogueHeader.text = _headerText;
            
            if (_isShowing)
            {
                DisplayText();
                return this;
            }
            _dialogueBox.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f)
                .OnComplete(DisplayText);
            
            return this;
        }

        private void DisplayText()
        {
            PlayAudio();
            _isShowing = true;
            _dialogueBody.text = "";
            _currentTween = _dialogueBody.DOText(_bodyText, _textDisplayDuration)
                .OnComplete(() => _isComplete = true);
        }

        private void PlayAudio()
        {
            _audioSource.Play();
        }

        private void Clean(bool isShowing = false)
        {
            if (!isShowing) _dialogueHeader.text = string.Empty;
            _dialogueBody.text = string.Empty;
            if (!isShowing) _headerText = string.Empty;
            _bodyText = string.Empty;
            _isShowing = isShowing;
            _isComplete = false;
            _shouldStayOnScreen = false;
            _textDisplayDuration = 0f;
            _currentTween = null;
            _callback = null;
        }
        
    }
}
