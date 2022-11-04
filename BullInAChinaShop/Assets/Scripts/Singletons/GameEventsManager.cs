using System;
using System.Collections.Generic;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Utils;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class GameEventsManager : Singleton<GameEventsManager>
    {
        private readonly Dictionary<GameEvent, Action<Dictionary<string, object>>> _eventDictionary
            = new Dictionary<GameEvent, Action<Dictionary<string, object>>>();

        public void AddListener(GameEvent e, Action<Dictionary<string, object>> listener)
        {
            if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
            {
                thisEvent += listener;
                _eventDictionary[e] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary.Add(e, thisEvent);
            }
        }

        public void RemoveListener(GameEvent e, Action<Dictionary<string, object>> listener)
        {
            if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
            {
                thisEvent -= listener;
                _eventDictionary[e] = thisEvent;
            }
            else
            {
                Debug.LogWarning($"Attempting to remove listener from a null event. Event: {e} - Listener: {listener.Method}");
            }
        }

        public void TriggerEvent(GameEvent e, Dictionary<string, object> message)
        {
            if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
            {
                thisEvent?.Invoke(message);
            }
        }
    }
}