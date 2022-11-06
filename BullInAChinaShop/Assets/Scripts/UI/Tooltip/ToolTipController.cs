using CharaGaming.BullInAChinaShop.UI.Utils;
using DG.Tweening;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.UI.Tooltip
{
    public class ToolTipController : MonoBehaviour
    {

        private RectTransform _transform;
        private FollowMouse _followMouse;
        private Sequence _currentSeq;
        
        [SerializeField]
        private StickyCanvas _stickyCanvas;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [field: SerializeField]
        public GameObject ToolTip { get; set; }

        [SerializeField]
        [Range(0f, 0.3f)]
        private float _edgeDetectionRadius = 0.15f;
        
        [SerializeField]
        [Range(0f, 0.1f)]
        private float _fadeInDistance = 0.05f;
        
        [SerializeField]
        [Range(0.1f, 1f)]
        private float _fadeInDuration = 0.25f;
        
        [SerializeField]
        [Range(0.1f, 1f)]
        private float _fadeInDelay = 0.35f;

        [SerializeField]
        [Range(0f, 0.1f)]
        private float _defaultXOffset;

        public void Activate(GameObject activatingObj, ToolTipInfo[] toolTipInfos, bool isFixed = true)
        {
            if (toolTipInfos.Length == 0) return;

            _transform ??= GetComponent<RectTransform>();

            if (isFixed)
            {
                if (_followMouse != null) Destroy(_followMouse);
                _followMouse = null;
                ActivateFixed(activatingObj, toolTipInfos);
                return;
            }

            _followMouse ??= GetComponent<FollowMouse>() ?? gameObject.AddComponent<FollowMouse>();
            _followMouse.Offset = new Vector2(_defaultXOffset * Screen.width, 0);

            foreach (var t in toolTipInfos)
            {
                new ToolTipBuilder(t)
                    .SetController(this)
                    .Build();
            }
        }

        private void ActivateFixed(GameObject activatingObj, params ToolTipInfo[] toolTipInfos)
        {
            var objRect = activatingObj.GetComponent<RectTransform>();
            CalculatePivotAndPosition(objRect);
            foreach (var t in toolTipInfos)
            {
                new ToolTipBuilder(t)
                    .SetController(this)
                    .Build();
            }
        }

        private void CalculatePivotAndPosition(RectTransform rect)
        {
            _stickyCanvas ??= GetComponentInParent<StickyCanvas>();
            var cam = _stickyCanvas.StuckCanvas.worldCamera != null ? _stickyCanvas.StuckCanvas.worldCamera : Camera.main;
            if (cam == null)
            {
                Debug.LogError($"Fixed ToolTipController requires a camera in the scene!");
                return;
            }

            var xOffset = _defaultXOffset * Screen.width;
            
            var objScreenCorners = rect.GetScreenCorners(cam);
            var objScreenPos = cam.WorldToScreenPoint(rect.position);
            var (objScreenWidth, objScreenHeight) = UIExtensions.GetScreenSize(objScreenCorners);
            var objPivot = rect.pivot;

            var objXOffset = objScreenWidth * (1f - objPivot.x);
            var objYOffset = objScreenHeight * (0.5f - objPivot.y);
            

            var xEdgeThreshold = Screen.width * _edgeDetectionRadius;
            var yEdgeThreshold = Screen.height * _edgeDetectionRadius;
            
            var tooCloseToRightEdge = objScreenCorners[3].x >= Screen.width - xEdgeThreshold;

            var tooCloseToTopEdge = objScreenCorners[1].y >= Screen.height - yEdgeThreshold;
            var tooCloseToBottomEdge = objScreenCorners[0].y <= yEdgeThreshold;

            var pivot = new Vector2(0f, 0.5f);

            if (tooCloseToRightEdge)
            {
                pivot.x = 1f;
                objXOffset *= -1;
                xOffset *= -1;
            }

            if (tooCloseToTopEdge)
            {
                pivot.y = 1f;
            }
            else if (tooCloseToBottomEdge)
            {
                pivot.y = 0f;
            }

            _transform.pivot = pivot;
            
            var screenPos = new Vector3(objScreenPos.x + objXOffset + xOffset, objScreenPos.y + objYOffset, objScreenPos.z);
            var fadeYOffset = tooCloseToBottomEdge
                ? _fadeInDistance * Screen.height
                : _fadeInDistance * Screen.height * -1;
            
            var fadeEndPos = cam.ScreenToWorldPoint(screenPos);
            var fadeStartPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y + fadeYOffset, screenPos.z));

            _currentSeq = DOTween.Sequence();
            _currentSeq.PrependInterval(_fadeInDelay);
            _currentSeq.Append(_transform.DOMove(fadeEndPos, _fadeInDuration).From(fadeStartPos));
            _currentSeq.Insert(_fadeInDelay, _canvasGroup.DOFade(1f, _fadeInDuration).From(0f));
        }
        
        public void DeActivate()
        {
            _currentSeq.Kill();
            transform.DestroyAllChildren();
        }
    }
}