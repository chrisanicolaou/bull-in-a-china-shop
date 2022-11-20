using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.MainMenu
{
    public class WordSlammer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _audioClip;

        [SerializeField]
        private bool _playAudio = true;
        
        [SerializeField]
        private TextMeshProUGUI[] _words;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _slamDuration = 0.1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _slamDelay = 0.1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _slamInterval = 0.1f;

        [SerializeField]
        private Ease _slamType = Ease.OutQuad;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _shakeDuration = 0.1f;

        [SerializeField]
        private float _startScale = 0.1f;

        private void Start()
        {
            SlamWords();
        }

        public void SlamWords()
        {
            var seq = DOTween.Sequence();
            seq.PrependInterval(_slamDelay);
            foreach (var word in _words)
            {
                word.gameObject.SetActive(false);
                seq.AppendInterval(_slamInterval);
                seq.AppendCallback(() => word.gameObject.SetActive(true));
                seq.Append(SlamWord(word));
            }
        }

        private TweenerCore<Vector3, Vector3, VectorOptions> SlamWord(TextMeshProUGUI word)
        {
            return word.DOScale(1f, _slamDuration).From(_startScale).SetEase(_slamType)
                .OnComplete(() =>
                {
                    _audioSource.loop = false;
                    _audioSource.PlayOneShot(_audioClip);
                    word.transform.DOShakeScale(_shakeDuration);
                });
        }
    }
}
