using MET.Applications.Events;
using MET.Core.Manager;
using TMPro;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.UI
{
    public class BestScoreVisualizer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreField;

        void Start()
        {
            EventBus.Me.Subscribe<OnSaveLoaded>(OnSaveLoaded);
        }

        private void OnSaveLoaded(OnSaveLoaded args)
        {
            SetPoint();
        }

        public void SetPoint()
        {
            _scoreField.text = $"Best Score: {DataManager.Me.GetMaxPoint()}";
        }
    }
}