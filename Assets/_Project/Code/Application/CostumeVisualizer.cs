using MET.Core.Applications;
using UnityEngine;

namespace MET.UI
{
    public abstract class CostumeVisualizer : MonoBehaviour
    {
        [SerializeField] protected float _frameRate = 0.1f;

        private BirdCostumeSAO _spriteList;
        private int _currentSpriteIndex;

        private float _timer;
        private int _direction = 1;
        private bool _isStarted;

        private void Update()
        {
            if (!_isStarted || _spriteList == null) return;

            _timer -= TimerManager.Delta;

            if (_timer <= 0)
            {
                UpdateIndex();
                _timer = _frameRate;
                SetSprite();
            }
        }

        private void UpdateIndex()
        {
            int maxIndex = _spriteList.Sprites.Length - 1;

            _currentSpriteIndex += _direction;

            if (_currentSpriteIndex >= maxIndex || _currentSpriteIndex <= 0)
            {
                _direction *= -1; 
            }

            _currentSpriteIndex = Mathf.Clamp(_currentSpriteIndex, 0, maxIndex);
        }

        public abstract void SetSprite();

        protected Sprite GetSprite()
        {
            return _spriteList.Sprites[_currentSpriteIndex];
        }

        public void StartShow(BirdCostumeSAO costume)
        {
            _spriteList = costume;
            _currentSpriteIndex = 0;
            _direction = 1;
            _timer = _frameRate;
            _isStarted = true;
            SetSprite();
        }
    }
}