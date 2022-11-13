using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.Utils;
using DG.Tweening;
using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private int _cash = 500;

        public int Cash
        {
            get => _cash;
            set
            {
                if (_cash == value) return;
                _cash = value;
                GameEventsManager.Instance.TriggerEvent(GameEvent.CashChanged, null);
            }
        }

        public List<BaseStock> AvailableStock { get; private set; }

        [field: SerializeField]
        public int DayNum { get; set; } = 1;

        [field: SerializeField]
        public int MinCustomers { get; set; } = 20;

        [field: SerializeField]
        public float MinTimeBetweenSpawn { get; set; } = 5f;

        [field: SerializeField]
        public float ShopperThinkTime { get; set; } = 5f;

        [field: SerializeField]
        public float ShopperServeTime { get; set; } = 3f;

        [field: SerializeField]
        public float ShopperImpatienceTime { get; set; } = 15f;

        [field: SerializeField]
        public int NumOfDaysBetweenBullEncounters { get; set; } = 3;
        
        public DayStats DayStats { get; set; }

        protected override void Awake()
        {
            base.Awake();
            DOTween.Init();
            PopulateAvailableStock();
        }

        private void PopulateAvailableStock()
        {
            var type = typeof(BaseStock);
            AvailableStock = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c =>
                    c != type &&
                    c.IsClass &&
                    type.IsAssignableFrom(c))
                .Select(t => (BaseStock)Activator.CreateInstance(t))
                .OrderBy(s => !s.IsUnlocked).ToList();
        }
    }
}
