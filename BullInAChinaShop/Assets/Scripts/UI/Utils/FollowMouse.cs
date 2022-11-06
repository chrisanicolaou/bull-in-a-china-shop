using UnityEngine;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class FollowMouse : MonoBehaviour
    {
        [field: SerializeField]
        public Vector2 Offset { get; set; }

        private RectTransform _canvasRect;
        private RectTransform _rectTransform;

        private Camera _cam;

        private void Awake()
        {
            _canvasRect = transform.root.GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
            _cam = Camera.main;
            if (_cam == null)
            {
                Debug.LogError("Follow mouse cannot work without a main camera!");
                Destroy(this);
                return;
            }

            transform.position = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + Offset.x,
                Input.mousePosition.y + Offset.y, Mathf.Abs(_cam.transform.position.z - transform.position.z)));
        }

        private void Update()
        {
            transform.position = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + Offset.x, Input.mousePosition.y + Offset.y,
                Mathf.Abs(_cam.transform.position.z - transform.position.z)));
        }
    }
}