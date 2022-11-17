using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public static class UIExtensions
    {
        public static Button AddButton(this GameObject obj, Selectable.Transition transition = Selectable.Transition.None)
        {
            var btn = obj.GetComponent<Button>() ?? obj.AddComponent<Button>();
            btn.transition = transition;
            return btn;
        }
        
        public static void DestroyAllChildren(this Transform transform)
        {
            var childCount = transform.childCount;
            if (childCount > 0)
            {
                for (int i = childCount - 1; i > -1; i--)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
        {
            var tChildren = parent.GetComponentsInChildren<Transform>();
            var targetChild = tChildren.FirstOrDefault(c => c.gameObject.CompareTag(tag));
            return targetChild == null ? null : targetChild.GetComponent<T>();
        }

        public static Vector3[] GetScreenCorners(this RectTransform rect, Camera cam)
        {
            cam ??= Camera.main;
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);

            return corners.Select(cam.WorldToScreenPoint).ToArray();
        }

        public static (float width, float height) GetScreenSize(Vector3[] corners)
        {
            return (GetScreenWidth(corners), GetScreenHeight(corners));
        }
        
        public static (float width, float height) GetScreenSize(this RectTransform rect, Camera cam)
        {
            var corners = rect.GetScreenCorners(cam);
            return (GetScreenWidth(corners), GetScreenHeight(corners));
        }

        public static float GetScreenWidth(Vector3[] corners)
        {
            return corners[2].x - corners[0].x;
        }

        public static float GetScreenHeight(Vector3[] corners)
        {
            return corners[2].y - corners[0].y;
        }

        public static string ToTMProColor(this string inputStr, Color color)
        {
            var htmlColor = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{htmlColor}>{inputStr}</color>";
        }
    }
}