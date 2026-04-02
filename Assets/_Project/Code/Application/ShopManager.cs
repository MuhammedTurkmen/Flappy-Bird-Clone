using System.Collections.Generic;
using MET.Applications.Events;
using MET.Core.Manager;
using MET.Core.Patterns;
using MET.Core.Types;
using TMPro;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.UI
{
    public class ShopManager : Singleton<ShopManager, IShopManager> , IShopManager
    {
        public bool IsCharacterSelectionValid { get; private set; }

        [SerializeField] private BirdCostumeSAO[] _costumes;

        [SerializeField] private GameObject _lockedArea;
        [SerializeField] private GameObject _unlockedText;
        [SerializeField] private GameObject _lockedText;

        [SerializeField] private TextMeshProUGUI _indexText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private int _costumeIndex;
        private List<int> _buyedCostumes = new();

        // Components

        private CostumeVisualizer _costumeVisualizer;

        private void OnEnable()
        {
            if (_buyedCostumes != null && _buyedCostumes.Count > 0)
            {
                SetLockedInterface();
            }
        }

        private void Start()
        {
            EventBus.Me.Subscribe<OnSaveLoaded>(OnSaveLoaded);
            EventBus.Me.Subscribe<OnGameReset>(OnGameReset);

            _costumeVisualizer = GetComponentInChildren<CostumeVisualizer>();
        }

        private void OnSaveLoaded(OnSaveLoaded args)
        {
            _costumeIndex = DataManager.Me.allData.CostumeData.SelectedCostumeIndex;
            _buyedCostumes = DataManager.Me.allData.CostumeData.BuyedCostumeIndexes;
            SetLockedInterface();
        }

        private void OnGameReset(OnGameReset args)
        {
            int maxPoint = DataManager.Me.GetMaxPoint();

            foreach (var item in _costumes)
            {
                if (item.Point <= maxPoint)
                {
                    _buyedCostumes.Add(item.ID);
                }
            }

            DataManager.Me.allData.CostumeData.BuyedCostumeIndexes = _buyedCostumes;
        }

        public BirdCostumeSAO GetCostumeSet()
        {
            if (_costumeIndex > _costumes.Length - 1 || _costumeIndex < 0) return null;

            return _costumes[_costumeIndex];
        }

        public void ShowNext()
        {
            if (_costumeIndex >= _costumes.Length - 1) return;

            _costumeIndex++;

            SetLockedInterface();
        }

        public void ShowPrevious()
        {
            if (_costumeIndex <= 0) return;

            _costumeIndex--;

            SetLockedInterface();
        }

        private void SetIndexText()
        {
            _indexText.text = $"{_costumeIndex}/{_costumes.Length - 1}";
        }

        private void SetLockedInterface()
        {
            SetIndexText();

            BirdCostumeSAO costumeSAO = GetCostumeSet();

            _costumeVisualizer.StartShow(costumeSAO);

            bool isBuyed = _buyedCostumes.Contains(costumeSAO.ID);

            IsCharacterSelectionValid = isBuyed;

            if (!isBuyed)
                _scoreText.text = $"SCORE {costumeSAO.Point}";

            _unlockedText.SetActive(isBuyed);
            _lockedText.SetActive(!isBuyed);
            _lockedArea.SetActive(!isBuyed);
        }
    }
}