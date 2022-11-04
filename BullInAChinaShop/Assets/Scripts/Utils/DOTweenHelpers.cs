using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Utils
{
    // ReSharper disable once InconsistentNaming - DOTween is correct
    public static class DOTweenHelpers
    {
        public static TweenChain PulseColor(this Material mat, Color startCol, Color endCol, float duration, bool startAtMax = false)
        {
            var tweenChain = new TweenChain();
            var (startColor, endColor) = startAtMax ? (endCol, startCol) : (startCol, endCol);
            var tween = mat.DOColor(endColor, duration);
            tween.OnComplete(() =>
            {
                mat.Pulse(tweenChain, startColor, endColor, duration);
            });
            tweenChain.AddToQueue(tween);
            return tweenChain;
        }

        private static void Pulse(this Material mat, TweenChain tweenChain, Color startCol, Color endCol, float duration, bool startAtMax = false)
        {
            var (startColor, endColor) = startAtMax ? (endCol, startCol) : (startCol, endCol);
            var tween = mat.DOColor(endColor, duration);
            tween.OnComplete(() =>
                {
                    mat.Pulse(tweenChain, startColor, endColor, duration, !startAtMax);
                });
            tweenChain.AddToQueue(tween);
        }
    }
}
