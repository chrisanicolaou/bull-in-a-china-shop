using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class HoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [field: SerializeField]
        public Texture2D NewCursor { get; set; }

        [field: SerializeField]
        public Vector2 HotSpot { get; set; }

        [field: SerializeField]
        public CursorMode Mode { get; set; }

        public bool IsDisabled { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null) return;
            if (IsDisabled) return;
            Cursor.SetCursor(NewCursor, HotSpot, Mode);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        }

        private void OnDestroy()
        {
            Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        }
    }
}