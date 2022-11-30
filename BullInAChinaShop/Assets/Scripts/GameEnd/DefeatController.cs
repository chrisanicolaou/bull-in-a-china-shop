using System;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.GameEnd
{
    public class DefeatController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _uhOhText;
        
        [SerializeField]
        private CanvasGroup _uhOhFader;
        
        [SerializeField]
        private CanvasGroup _descriptionFader;
        
        [SerializeField]
        private CanvasGroup _quitButtonFader;
        
        [SerializeField]
        private Button _quitButton;
        
        [SerializeField]
        private CanvasGroup _sceneFader;

        private void Start()
        {
            var seq = DOTween.Sequence();
            seq.Append(_sceneFader.DOFade(0f, 1f));
            seq.Append(_uhOhFader.DOFade(1f, 0.7f));
            Vector2 uhOhPos = _uhOhText.anchoredPosition;
            seq.Append(_uhOhText.DOAnchorPos(new Vector2(uhOhPos.x, uhOhPos.y + 82f), 0.8f).SetEase(Ease.OutQuad));
            seq.AppendInterval(0.5f);
            seq.Append(_descriptionFader.DOFade(1f, 0.7f));
            seq.AppendCallback(() => _quitButton.onClick.AddListener(() => WebGLHelpers.Quit()));
            seq.AppendInterval(2f);
            seq.Append(_quitButtonFader.DOFade(1f, 0.7f));
        }
    }
}
