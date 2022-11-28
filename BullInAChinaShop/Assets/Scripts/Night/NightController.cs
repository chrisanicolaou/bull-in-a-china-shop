using System;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.UI.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Night
{
    public class NightController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _headerText;

        [SerializeField]
        private TextMeshProUGUI _cashEarnedText;
        
        [SerializeField]
        private TextMeshProUGUI _cashEarnedNumText;

        [SerializeField]
        private TextMeshProUGUI _shoppersServedText;
        
        [SerializeField]
        private TextMeshProUGUI _shoppersServedNumText;
        
        [SerializeField]
        private Transform _reviewPanel;

        [SerializeField]
        private GameObject _reviewPrefab;

        [SerializeField]
        private Sprite _badReviewBackground;

        [SerializeField]
        private Sprite _goodReviewBackground;

        [SerializeField]
        private CanvasGroup _continueButtonCanvasGroup;

        [SerializeField]
        private Button _continueButton;

        private void Start()
        {
            var seq = DOTween.Sequence();
            var stats = GameManager.Instance.DayStats ?? new DayStats();
            seq.AppendInterval(1f);
            seq.Append(_headerText.DOText($"Day {GameManager.Instance.DayNum} Complete", 0.3f));
            seq.Append(_cashEarnedText.DOText("Cash earned:", 0.3f));
            seq.Insert(1.3f, _shoppersServedText.DOText("Shoppers served:", 0.3f));
            seq.Append(_cashEarnedNumText.DOText(stats.CashEarned.ToString(), 1f, scrambleMode: ScrambleMode.Numerals));
            seq.Insert(1.6f, _shoppersServedNumText.DOText(stats.ShoppersServed.ToString(), 1f, scrambleMode: ScrambleMode.Numerals));

            var reviewCount = 0;

            foreach (var review in stats.Reviews)
            {
                if (reviewCount == 6) break;
                reviewCount++;
                var shopperReviewObj = Instantiate(_reviewPrefab, _reviewPanel);
                var canvasGroup = shopperReviewObj.GetComponent<CanvasGroup>();
                var backgroundImg = shopperReviewObj.FindComponentInChildWithTag<Image>("ReviewBackgroundIcon");
                backgroundImg.sprite = review.Type == ReviewType.Happy ? _goodReviewBackground : _badReviewBackground;
                var img = shopperReviewObj.FindComponentInChildWithTag<Image>("ReviewShopperIcon");
                img.sprite = Resources.Load<Sprite>(review.ReviewSpriteFilePath);
                var reviewText = shopperReviewObj.GetComponentInChildren<TextMeshProUGUI>();
                seq.Append(canvasGroup.DOFade(1f, 0.2f)
                    .OnComplete(() =>
                    {
                        reviewText.DOText(review.ReviewText, 0.6f);
                    }));
            }

            seq.AppendInterval(1f);
            seq.Append(_continueButtonCanvasGroup.DOFade(1f, 0.7f));

            seq.OnComplete(() => _continueButton.onClick.AddListener(OnContinueButtonPress));
        }

        private void OnContinueButtonPress()
        {
            // Handle whatever needs to change before the next day
            GameManager.Instance.DayNum++;
            _continueButton.onClick.RemoveAllListeners();
            SceneFader.Instance.FadeToScene("Day"); //!!!! - CHANGE THIS PRE RELEASE !!!!!
        }
    }
}
