using System.Collections.Generic;
using MET.Applications.Events;
using MET.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using static MET.Applications.Events.GameEvents;

namespace MET.UI
{
    public class PointVisualiser : MonoBehaviour
    {
        [SerializeField] private GameObject _numberPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private Sprite[] _numberSprites;

        private List<Image> _activeImages = new();

        private void Start()
        {
            EventBus.Me.Subscribe<OnPointTaken>(OnPointTaken);
            //UpdateVisuals(0);
        }

        private void OnPointTaken(OnPointTaken args)
        {
            int point = GameManager.Me.GetPoint();
            UpdateVisuals(point);
        }

        public void UpdateVisuals(int value)
        {
            string pointStr = value.ToString();
            int requiredDigits = pointStr.Length;

            while (_activeImages.Count < requiredDigits)
            {
                GameObject newDigit = Instantiate(_numberPrefab, _container);
                Image img = newDigit.GetComponent<Image>();
                if (img != null)
                {
                    _activeImages.Add(img);
                }
            }

            for (int i = 0; i < _activeImages.Count; i++)
            {
                if (i < requiredDigits)
                {
                    _activeImages[i].gameObject.SetActive(true);

                    int digitValue = int.Parse(pointStr[i].ToString());

                    _activeImages[i].sprite = _numberSprites[digitValue];
                }
                else
                {
                    _activeImages[i].gameObject.SetActive(false);
                }
            }
        }
    }
}