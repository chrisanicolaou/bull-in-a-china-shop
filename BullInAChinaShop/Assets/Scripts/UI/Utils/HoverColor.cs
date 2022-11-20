using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class HoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _img;
        private Color _originalCol;
        private TweenerCore<Color, Color, ColorOptions> _currentTween;

        [SerializeField]
        private Color _targetCol;
        
        [SerializeField]
        [Range(0.1f, 1f)]
        private float _duration = 0.2f;

        private void Start()
        {
            _img = GetComponent<Image>();
            _originalCol = _img.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _currentTween?.Kill();
            _currentTween = _img.DOColor(_targetCol, _duration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _currentTween?.Kill();
            _currentTween = _img.DOColor(_originalCol, _duration);
        }
    }
}