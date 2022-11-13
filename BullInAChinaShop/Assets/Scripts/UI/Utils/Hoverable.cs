using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CharaGaming.BullInAChinaShop.UI.Utils
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Action _onHover;

        private Action _onExit;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _onHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onExit?.Invoke();
        }

        public void OnHover(Action callback)
        {
            _onHover = callback;
        }

        public void OnExit(Action callback)
        {
            _onExit = callback;
        }

        public void Clear()
        {
            _onHover = null;
            _onExit = null;
        }
    }
}