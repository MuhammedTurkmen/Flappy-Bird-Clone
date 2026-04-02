using MET.Applications.Events;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.Core
{
    public class GroundAnimationController : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();

            EventBus.Me.Subscribe<OnGameStarted>(OnGameStarted);
            EventBus.Me.Subscribe<OnGameEnd>(OnGameEnd);
        }

        public void OnGameStarted(OnGameStarted args)
        {
            _animator.Play("Slide");
        }

        public void OnGameEnd(OnGameEnd args)
        {
            _animator.Play("Idle");
        }
    }
}
