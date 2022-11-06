using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.UI.Tooltip
{
    public class ToolTipLoader : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField]
        private bool _testMode = false;

        [SerializeField]
        private ToolTipController _toolTipController;

        public ToolTipInfo[] ToolTipInfos { get; set; }

        public Action BeforeLoadHandler;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null) return;
            BeforeLoadHandler?.Invoke();
            if (_toolTipController == null)
            {
                var obj = GameObject.FindGameObjectWithTag("ToolTipController");
                if (obj == null)
                {
                    Debug.LogError("Could not find ToolTipController in scene! Have you added a tag?");
                    return;
                }
                _toolTipController = obj.GetComponent<ToolTipController>();
                if (_toolTipController == null)
                {
                    Debug.LogError("Could not find ToolTipController component on ToolTipController! Check the component exists.");
                    return;
                }
            }
            if (_testMode)
            {
                GenerateSampleToolTips();
            }
            _toolTipController.Activate(gameObject, ToolTipInfos);
        }

        private void GenerateSampleToolTips()
        {
            var random = new System.Random();
            ToolTipInfos = Enumerable.Range(1, random.Next(1, 5)).Select(c =>
            {
                var toolTipInfo = new ToolTipInfo("body", "Header", "");
                var bodyTextArr = Enumerable.Range(1, random.Next(1, 5)).Select(c => "This is a sample tooltip.").ToArray();
                toolTipInfo.Body = string.Join(" ", bodyTextArr);
                return toolTipInfo;
            }).ToArray();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_toolTipController)
            {
                _toolTipController.DeActivate();
            }
        }

        private void OnDestroy()
        {
            if (_toolTipController)
            {
                _toolTipController.DeActivate();
            }
        }
    }
}