using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class PulseFade : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        [Range(0f, 2f)]
        private float _duration;

        private IEnumerator _pulseCoroutine;

        private void OnEnable()
        {
            _pulseCoroutine ??= Pulse();
            _canvasGroup ??= GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            StartCoroutine(_pulseCoroutine);
        }

        private void OnDisable()
        {
            StopCoroutine(_pulseCoroutine);
            _canvasGroup.alpha = 0;
        }

        private IEnumerator Pulse()
        {
            var toMax = true;
            while (gameObject.activeSelf)
            {
                var endIntensity = toMax ? 1 : 0;

                var tween = _canvasGroup.DOFade(endIntensity, _duration).SetEase(Ease.InQuad);

                yield return tween.WaitForCompletion();
                toMax = !toMax;
            }
        }
    }
}