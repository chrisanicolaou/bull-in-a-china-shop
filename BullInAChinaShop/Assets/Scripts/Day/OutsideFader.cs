using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class OutsideFader : MonoBehaviour
    {
        [SerializeField]
        private Image[] _orderedImagesToFade;

        private void Start()
        {
            foreach (var img in _orderedImagesToFade)
            {
                img.enabled = true;
            }
        }

        public void StartFade(float dayDuration)
        {
            var fadeIncrementDuration = dayDuration / _orderedImagesToFade.Length;
            var seq = DOTween.Sequence();
            foreach (var img in _orderedImagesToFade)
            {
                seq.Append(img.DOColor(new Color(1f, 1f, 1f, 0f), fadeIncrementDuration));
            }
        }
    }
}
