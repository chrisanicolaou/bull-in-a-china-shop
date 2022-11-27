using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CharaGaming.BullInAChinaShop.Day;
using CharaGaming.BullInAChinaShop.Enums;
using CharaGaming.BullInAChinaShop.Stock;
using CharaGaming.BullInAChinaShop.Upgrades;
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
        
        public List<BaseUpgrade> Upgrades { get; private set; }

        public List<BaseStock> AvailableStock { get; private set; }

        [field: SerializeField]
        public int LoanAmount { get; set; } = 50000;

        [field: SerializeField]
        public int DayNum { get; set; } = 1;

        [field: SerializeField]
        public int DayDuration { get; set; } = 45;

        [field: SerializeField]
        public int ShopperPurchaseAmount { get; set; } = 1;

        [SerializeField]
        private float _spawnTime = 5f;

        public float SpawnTime
        {
            get => _spawnTime * SpawnTimeMultiplier;
            set => _spawnTime = value;
        }

        public float SpawnTimeMultiplier { get; set; } = 1.0f;

        [field: SerializeField]
        public float SpawnTimeVariance { get; set; } = 4f;

        [field: SerializeField]
        public float ShopperServeTime { get; set; } = 3f;

        [SerializeField]
        private float _shopperImpatienceTime;

        public float ShopperImpatienceTime
        {
            get => _shopperImpatienceTime * ShopperImpatienceTimeMultiplier;
            set => _shopperImpatienceTime = value;
        }

        public float ShopperImpatienceTimeMultiplier { get; set; } = 1.0f;

        [SerializeField]
        private float _shopperThinkTime;

        public float ShopperThinkTime
        {
            get => _shopperThinkTime * ShopperThinkTimeMultiplier;
            set => _shopperThinkTime = value;
        }

        public float ShopperThinkTimeMultiplier { get; set; } = 1.0f;

        [field: SerializeField]
        public int NumOfDaysBetweenBullEncounters { get; set; } = 5;

        [field: SerializeField]
        public int TotalNumOfDays { get; set; } = 30;
        
        [field: SerializeField]
        public GameObject CurrentTill { get; set; }

        public int DaysUntilNextBullEncounter => BullEncounterDays.FirstOrDefault(d => d > DayNum) - DayNum;
        
        public DayStats DayStats { get; set; }

        public List<int> BullEncounterDays { get; set; }
        
        [field: SerializeField]
        public Texture2D DefaultCursor { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            DOTween.Init();
            PopulateAvailableStock();
            PopulateUpgrades();
            CalculateBullEncounterDays();
            Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        }

        private void PopulateUpgrades()
        {
            var type = typeof(BaseUpgrade);
            Upgrades = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c =>
                    c != type &&
                    c.IsClass &&
                    type.IsAssignableFrom(c))
                .Select(t => (BaseUpgrade)Activator.CreateInstance(t))
                .OrderBy(s => s.UpgradeCost).ToList();
        }

        private void CalculateBullEncounterDays()
        {
            BullEncounterDays = new List<int> { 1 };

            for (var i = NumOfDaysBetweenBullEncounters; i <= TotalNumOfDays; i += NumOfDaysBetweenBullEncounters)
            {
                BullEncounterDays.Add(i);
            }
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
                .OrderByDescending(s => s.IsUnlocked)
                .ThenBy(s => s.SellValue).ToList();
        }
    }
}
