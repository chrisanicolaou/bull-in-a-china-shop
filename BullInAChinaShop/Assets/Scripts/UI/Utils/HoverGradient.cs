using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class HoverGradient : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Color _colorTwo = Color.black;
        [SerializeField]
        private float _duration = 0.5f;

        private Image _image;
        private Color _originalColor;
        
        private readonly Gradient _targetGradient = new Gradient();
        private readonly Gradient _originalGradient = new Gradient();
        private readonly GradientColorKey[] _originalGradientColorKeys = new GradientColorKey[2];
        private readonly GradientColorKey[] _targetGradientColorKeys = new GradientColorKey[2];
        private readonly GradientAlphaKey[] _alphaKeys = new GradientAlphaKey[2];

        private void Start()
        {
            _image = GetComponent<Image>();
            _originalColor = _image.color;

            _targetGradientColorKeys[0].color = _originalColor;
            _targetGradientColorKeys[0].time = 0.0f;
            _targetGradientColorKeys[1].color = _colorTwo;
            _targetGradientColorKeys[1].time = 1.0f;

            _originalGradientColorKeys[0].color = _colorTwo;
            _originalGradientColorKeys[0].time = 0.0f;
            _originalGradientColorKeys[1].color = _originalColor;
            _originalGradientColorKeys[1].time = 1.0f;

            _alphaKeys[0].alpha = 0f;
            _alphaKeys[0].time = 0.0f;
            _alphaKeys[1].alpha = 0f;
            _alphaKeys[1].time = 1.0f;

            _targetGradient.SetKeys(_targetGradientColorKeys, _alphaKeys);
            _originalGradient.SetKeys(_originalGradientColorKeys, _alphaKeys);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.DOGradientColor(_targetGradient, _duration);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _image.DOGradientColor(_originalGradient, _duration)
                .OnComplete(() => _image.color = _originalColor);
        }
    }
}