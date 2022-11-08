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

            var duration = CalculateNormalizedDuration(rect.anchoredPosition, shopPos.VectorPos);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(shopPos.VectorPos, duration));
            seq.Insert(0f, rect.DOScale(shopPos.Scale, duration));

            return seq;
        }

        public Sequence JoinQueue(RectTransform rect)
        {
            var shopPos = _shopPositions.FirstOrDefault(p => p.Location == ShopLocation.DeskTill);
            
            if (shopPos == null)
            {
                Debug.LogError("Shop position not found!");
                return null;
            }

            var queuePos = new Vector2(shopPos.VectorPos.x - _queuePosXOffset * (_dayController.ShopperQueue.Count - 1), shopPos.VectorPos.y);
            var duration = CalculateNormalizedDuration(rect.anchoredPosition, queuePos);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(queuePos, duration));
            seq.Insert(0f, rect.DOScale(shopPos.Scale, duration));

            return seq;
        }

        public Sequence MoveTo(RectTransform rect, Vector2 location, float scale = 1f)
        {

            var duration = CalculateNormalizedDuration(rect.anchoredPosition, location);

            var seq = DOTween.Sequence();

            seq.Insert(0f, rect.DOAnchorPos(location, duration));
            seq.Insert(0f, rect.DOScale(scale, duration));

            return seq;
        }

        public Vector2 CalculateQueuePosition(int index)
        {
            return new Vector2(_tillPos.VectorPos.x - _queuePosXOffset * index, _tillPos.VectorPos.y);
        }

        private float CalculateNormalizedDuration(Vector3 currentPos, Vector3 targetPos)
        {
            var xDist = Mathf.Abs(currentPos.x - targetPos.x);
            var yDist = Mathf.Abs(currentPos.y - targetPos.y) * _yPerspectiveMultiplier;
            var travelDist = xDist + yDist;
            return Math.Min(travelDist * _normalizedDurationMultiplier, 2);
        }

        // private int CalculateSiblingIndex(ShopLocation location)
        // {
        //     var index = 0;
        //
        //     switch (location)
        //     {
        //         case ShopLocation.OutsideStart:
        //             index = 1;
        //             break;
        //         case ShopLocation.OutsideDoor:
        //             index = 1;
        //             break;
        //         case ShopLocation.InsideDoor:
        //             index = _shopCanvas.childCount - (2 + _dayController.ShopperQueue.Count + 1);
        //             break;
        //         case ShopLocation.DeskCenter:
        //             index = _shopCanvas.childCount - 2;
        //             break;
        //         case ShopLocation.DeskTill:
        //             index = _shopCanvas.childCount - (2 + _dayController.ShopperQueue.Count + 1);
        //             break;
        //         case ShopLocation.LoiterOne:
        //             index = _shopCanvas.childCount - (2 + _dayController.ShopperQueue.Count + 1);
        //             break;
        //         case ShopLocation.LoiterTwo:
        //             index = _shopCanvas.childCount - (2 + _dayController.ShopperQueue.Count + 1);
        //             break;
        //         case ShopLocation.LoiterThree:
        //             index = _shopCanvas.childCount - (2 + _dayController.ShopperQueue.Count + 1);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(location), location, null);
        //     }
        //
        //     return index;
        // }
    }
}