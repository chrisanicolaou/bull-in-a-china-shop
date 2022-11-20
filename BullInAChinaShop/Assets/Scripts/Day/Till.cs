using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Singletons;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class Till : MonoBehaviour
    {
        [SerializeField]
        private GameObject _upgradeTill;

        [SerializeField]
        private AudioSource _sfxController;

        [SerializeField]
        private AudioClip[] _sfxClips;

        [SerializeField]
        private GameObject _sellTip;

        [SerializeField]
        private RectTransform _sellTipTransform;

        [SerializeField]
        private Image _stockImg;

        [SerializeField]
        private TextMeshProUGUI _profit;

        [SerializeField]
        private TextMeshProUGUI _quantitySold;

        private Vector2 _startPos;

        private void Start()
        {
            GameEventsManager.Instance.AddListener(GameEvent.ItemSold, OnItemSold);
            _startPos = _sellTipTransform.anchoredPosition;
            _sfxController.loop = false;
        }

        public void Upgrade(int numOfTimes = 1)
        {
            if (numOfTimes > 1)
            {
                var newTill = Instantiate(_upgradeTill, transform.parent, false);
                var nextTill = newTill.GetComponent<Till>();
                Destroy(gameObject);
                nextTill.Upgrade(numOfTimes - 1);
            }
            else
            {
                Destroy(gameObject);
                var newTill = Instantiate(_upgradeTill, transform.parent, false);
                newTill.transform.localScale = new Vector3(0f, 0f, 0f);
                var seq = DOTween.Sequence();
                seq.PrependInterval(0.5f);
                seq.Append(newTill.transform.DOScale(1f, 0.5f));
                seq.OnComplete(() =>
                {
                    GameManager.Instance.CurrentTill = _upgradeTill;
                    var controller = GameObject.FindWithTag("DayController").GetComponent<DayController>();
                    controller.ReassignTill(newTill);
                });
            }
        }

        private void OnItemSold(Dictionary<string, object> message)
        {
            if (!message.TryGetValue("item", out object stockObj) || !message.TryGetValue("quantity", out object quantityObj))
            {
                Debug.LogError("Item or quantity key missing!");
                return;
            }

            var stock = (BaseStock)stockObj;
            var quantity = (int)quantityObj;
            var profitText = (stock.SellValue * quantity).KiloFormat();
            var quantityText = quantity.KiloFormat();

            _stockImg.sprite = Resources.Load<Sprite>(stock.SpriteFilePath);
            _profit.text = $"+<color=\"green\">{profitText} $</color>";
            _quantitySold.text = $"-<color=\"red\">{quantityText}</color>";

            _sellTip.SetActive(true);
            _sellTipTransform.DOAnchorPos(new Vector2(_startPos.x, _startPos.y + 20), 1.5f)
                .From(_startPos).SetEase(Ease.OutQuad)
                .OnComplete(() => { _sellTip.SetActive(false); });

            _sfxController.clip = _sfxClips[Random.Range(0, _sfxClips.Length)];
            _sfxController.Play();
        }

        private void OnDestroy()
        {
            GameEventsManager.Instance.RemoveListener(GameEvent.ItemSold, OnItemSold);
        }
    }
}