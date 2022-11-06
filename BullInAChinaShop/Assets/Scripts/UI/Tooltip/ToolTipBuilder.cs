using CharaGaming.BullInAChinaShop.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.Tooltip
{
    public class ToolTipBuilder
    {
        private string _header;

        private string _headerIconSpriteFilePath;

        private string _body;

        private ToolTipController _controller;

        public ToolTipBuilder()
        {

        }

        public ToolTipBuilder(ToolTipInfo toolTipInfo)
        {
            _header = toolTipInfo.Header;
            _headerIconSpriteFilePath = toolTipInfo.HeaderIconSpriteFilePath;
            _body = toolTipInfo.Body;
        }

        public ToolTipBuilder SetController(ToolTipController controller)
        {
            _controller = controller;
            return this;
        }

        public ToolTipBuilder SetHeader(string header)
        {
            _header = header;
            return this;
        }

        public ToolTipBuilder SetHeaderIcon(string headerIcon)
        {
            _headerIconSpriteFilePath = headerIcon;
            return this;
        }

        public ToolTipBuilder SetBody(string body)
        {
            _body = body;
            return this;
        }

        public GameObject Build()
        {
            if (_controller == null)
            {
                Debug.LogError("Cannot build tooltip - no controller set!");
                return null;
            }
            var toolTip = _controller.ToolTip != null ? _controller.ToolTip : Resources.Load<GameObject>("ToolTip");
            var toolTipObj = Object.Instantiate(toolTip, Vector3.zero, Quaternion.identity);
            if (toolTipObj == null)
            {
                Debug.LogError("Cannot find ToolTip prefab! Make sure it is named ToolTip!");
                return null;
            }
            toolTipObj.transform.SetParent(_controller.transform, false);
            if (!string.IsNullOrEmpty(_header))
            {
                toolTipObj.transform.GetChild(0).gameObject.SetActive(true);
                var headerText = toolTipObj.FindComponentInChildWithTag<TextMeshProUGUI>("ToolTipHeaderText");
                headerText.text = _header;
                if (!string.IsNullOrEmpty(_headerIconSpriteFilePath))
                {
                    toolTipObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    var icon = toolTipObj.FindComponentInChildWithTag<Image>("ToolTipIcon");
                    icon.sprite = Resources.Load<Sprite>(_headerIconSpriteFilePath);
                }
            }
            var bodyText = toolTipObj.FindComponentInChildWithTag<TextMeshProUGUI>("ToolTipBody");
            bodyText.text = _body;

            return toolTipObj;
        }
    }
}
