using MET.Applications.Events;
using MET.UI;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.Gameplay
{
    public class BirdAnimationPlayer : CostumeVisualizer
    {
        [SerializeField] private SpriteRenderer _spriteRend;

        private void Start()
        {
            EventBus.Me.Subscribe<OnGameStarted>(OnGameStarted);
        }

        private void OnGameStarted(OnGameStarted args)
        {
            StartShow(ShopManager.Me.GetCostumeSet());
        }

        public override void SetSprite()
        {
            _spriteRend.sprite = GetSprite();
        }

        public void ClearVisual()
        {
            _spriteRend.sprite = null;
        }
    }
}