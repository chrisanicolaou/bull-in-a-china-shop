namespace CharaGaming.BullInAChinaShop.UI.Tooltip
{
    public class ToolTipInfo
    {
        public string Header { get; set; }

        public string HeaderIconSpriteFilePath { get; set; }

        public string Body { get; set; }

        public ToolTipInfo()
        {

        }

        public ToolTipInfo(string body)
        {
            Body = body;
        }

        public ToolTipInfo(string body, string header) : this(body)
        {
            Header = header;
        }

        public ToolTipInfo(string body, string header, string headerIconSpriteFilePath) : this (body, header)
        {
            HeaderIconSpriteFilePath = headerIconSpriteFilePath;
        }
    }
}