using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class PulseLight : MonoBehaviour
    {
        [SerializeField]
        private Light2D _light;

        [SerializeField]
        private bool _startAtMax = true;
        
        [SerializeField]
        private Ease _ease = Ease.InQuad;

        [SerializeField]
        [Range(0f, 4f)]
        private float _minIntensity;

        [SerializeField]
        [Range(0f, 4f)]
        private float _maxIntensity;

        [SerializeField]
        [Range(0f, 2f)]
        private float _duration;

        [SerializeField]
        private float _startDelay;

        [SerializeField]
        private float _pulseInterval;

        private IEnumerator _pulseCoroutine;

        private void OnEnable()
        {
            _pulseCoroutine ??= Pulse();
            _light ??= GetComponent<Light2D>();
             _light.intensity = _startAtMax ? _maxIntensity : _minIntensity;
            StartCoroutine(_pulseCoroutine);
        }

        private void OnDisable()
        {
            StopCoroutine(_pulseCoroutine);
            _light.intensity = 0;
        }

        private IEnumerator Pulse()
        {
            var toMax = !_startAtMax;
            if (_startDelay > 0)
            {
                yield return new WaitForSeconds(_startDelay);
            }
            while (gameObject.activeSelf)
            {
                var endIntensity = toMax ? _maxIntensity : _minIntensity;

                var tween = DOTween.To(() => _light.intensity, (x) => _light.intensity = x, endIntensity, _duration).SetEase(_ease);

                yield return tween.WaitForCompletion();
                toMax = !toMax;

                if (_pulseInterval > 0 && toMax)
                {
                    yield return new WaitForSeconds(_pulseInterval);
                }
            }
        }
    }
}