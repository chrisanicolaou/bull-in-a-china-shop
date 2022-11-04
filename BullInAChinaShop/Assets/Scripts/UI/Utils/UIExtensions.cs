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
    }
}