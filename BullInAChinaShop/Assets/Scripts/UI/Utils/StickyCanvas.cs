using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class StickyCanvas : MonoBehaviour
    {
        [field: SerializeField]
        public Canvas StuckCanvas { get; set; }

        private void Start()
        {
            SetupCanvas();
        }

        private void SetupCanvas()
        {
            if (StuckCanvas == null)
            {
                StuckCanvas = GetComponent<Canvas>();
                if (StuckCanvas == null)
                {
                    Debug.LogWarning("StickyCanvas requires a Canvas component!");
                    return;
                }
            }
            StuckCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            StuckCanvas.worldCamera = Camera.main;
            SceneManager.activeSceneChanged += ResetCanvasCamera;
        }

        private void ResetCanvasCamera(Scene previous, Scene next)
        {
            StuckCanvas.worldCamera = Camera.main;
        }
    }
}