using System;
using System.Collections.Generic;
using System.Linq;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Day
{
    public class CharacterMover : MonoBehaviour
    {
        [SerializeField]
        private List<ShopPosition> _shopPositions = new List<ShopPosition>();

        private ShopPosition _tillPos;

        [SerializeField]
        [Range(0.001f, 0.05f)]
        private float _normalizedDurationMultiplier = 0.001f;
        
        [SerializeField]
        [Range(1f, 3f)]
        private float _yPerspectiveMultiplier = 1f;
        
        [SerializeField]
        private float _queuePosXOffset;

        [SerializeField]
        private DayController _dayController;

        [SerializeField]
        private Transform _shopCanvas;

        private void Start()
        {
            _tillPos = _shopPositions.FirstOrDefault(p => p.Location == ShopLocation.DeskTill);
        }

        public Sequence MoveTo(RectTransform rect, ShopLocation location)
        {
            var shopPos = _shopPositions.FirstOrDefault(p => p.Location == location);
            if (shopPos == null)
            {
                Debug.LogError("Shop position not found!");
                return null;
            }

            var (targetPos, targetScale) = shopPos.PosAndScale;
            RotateCharacter(rect, targetPos);

            var duration = CalculateNormalizedDuration(rect.anchoredPosition, targetPos);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(targetPos, duration));
            seq.Insert(0f, rect.DOScale(targetScale, duration));

            return seq;
        }

        public Sequence JoinQueue(RectTransform rect)
        {
            var (targetPos, targetScale) = _tillPos.PosAndScale;
            rect.rotation = Quaternion.Euler(0, 180, 0);

            var queuePos = new Vector2(targetPos.x - _queuePosXOffset * (_dayController.ShopperQueue.Count - 1), targetPos.y);
            var duration = CalculateNormalizedDuration(rect.anchoredPosition, queuePos);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(queuePos, duration));
            seq.Insert(0f, rect.DOScale(targetScale, duration));

            return seq;
        }

        public Sequence MoveTo(RectTransform rect, Vector2 location, float scale = 1f, bool reverseRotate = false)
        {
            RotateCharacter(rect, location, reverseRotate);
            var duration = CalculateNormalizedDuration(rect.anchoredPosition, location);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(location, duration));
            seq.Insert(0f, rect.DOScale(scale, duration));

            return seq;
        }

        private void RotateCharacter(RectTransform rect, Vector2 location, bool reverseRotate = false)
        {
            if (Mathf.Abs(location.x - rect.localPosition.x) < 0.0001f) return;
            var leftRotate = !reverseRotate ? 0 : 180;
            var rightRotate = !reverseRotate ? 180 : 0;
            rect.rotation = Quaternion.Euler(0, rect.localPosition.x < location.x ? rightRotate : leftRotate, 0);
        }

        public Vector2 CalculateQueuePosition(int index)
        {
            return new Vector2(_tillPos.PosAndScale.pos.x - _queuePosXOffset * index, _tillPos.PosAndScale.pos.y);
        }

        private float CalculateNormalizedDuration(Vector3 currentPos, Vector3 targetPos)
        {
            var xDist = Mathf.Abs(currentPos.x - targetPos.x);
            var yDist = Mathf.Abs(currentPos.y - targetPos.y) * _yPerspectiveMultiplier;
            var travelDist = xDist + yDist;
            return Math.Min(travelDist * _normalizedDurationMultiplier, 2);
        }
    }
}