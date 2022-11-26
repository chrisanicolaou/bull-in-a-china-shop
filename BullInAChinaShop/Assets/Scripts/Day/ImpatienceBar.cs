using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class ImpatienceBar : MonoBehaviour
    { 
        private Transform _shopperTransform { get; set; }

        private Transform _transform;

        private Slider _slider;

        private Image _img;

        private TweenerCore<float, float, FloatOptions> _slideTween;

        private void Awake()
        {
            _transform = transform;
            _shopperTransform = _transform.parent;
            _slider = GetComponent<Slider>();
            _img = GetComponent<Image>();
            _img.enabled = false;
            _slider.value = _slider.minValue;
        }

        private void Update()
        {
            _transform.rotation = Quaternion.Euler(0, _shopperTransform.rotation.y * -1, 0);
        }

        public void GetImpatient(float time)
        {
            _img.enabled = true;
            _slider.maxValue = 100;
            _slider.value = 100;
            _slideTween = _slider.DOValue(_slider.minValue + 10, time - time * 0.1f);
        }

        public void StopImpatienceBar()
        {
            _slideTween?.Kill();
            Destroy(gameObject);
        }
    }
}