using MET.Applications;
using MET.Applications.Events;
using MET.Core.Applications;
using MET.Core.Attributes;
using MET.Core.Manager;
using MET.Core.Types;
using MET.Core.Utilities.Extentions;
using MET.Core.Utilities.Helpers;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.Gameplay
{
    public class BirdMovement : MonoBehaviour
    {
        public BirdState State => _state;

        public bool MoveStarted { get; private set; }

        [Separator("Jump")]

        [SerializeField] private float _jumpForce = 7;

        [Separator("Rotation")]

        [SerializeField] private float _maxDownAngle = -60;
        [SerializeField] private float _maxUpAngle = 45;

        [SerializeField] private float _maxRiseSpeed = 7;
        [SerializeField] private float _maxFallSpeed = 10;

        [SerializeField][Range(0.05f, 1f)] private float _rotationSmoothTime = .5f;

        [Separator("Sound Effects")]

        [SerializeField] private AudioClip[] _hitClips;
        [SerializeField] private AudioClip[] _pointClips;
        [SerializeField] private AudioClip[] _wingClips;

        private BirdState _state;

        private bool IsFlying => _state == BirdState.Flying;
        private bool IsFalling => _state == BirdState.Falling;
        
        // --- Jump ---

        private const float _maxJumpWait = 0.15f;

        // --- Component ---

        private Rigidbody2D _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();

            _startPos = transform.position;
        }

        private Vector2 _startPos;

        private void OnEnable()
        {
            if (_startPos != Vector2.zero)
            {
                transform.position = _startPos;
            }
        }

        private void Start()
        {
            EventBus.Me.Subscribe<OnGameStarted>(OnGameStarted);
            EventBus.Me.Subscribe<OnGameReset>(OnGameReset);
        }

        private float _rotationVelocity;
        private float _waitTime;

        private void Update()
        {
            if (!GameManager.Me.IsGameStarted) return;

            float dt = TimerManager.Delta;

            if (_waitTime >= 0)
                _waitTime -= dt;

            if (InputManager.Me.JumpPressed && _waitTime <= 0 && IsFlying)
            {
                Jump(_jumpForce);
                _waitTime = _maxJumpWait;
            }

            if (MoveStarted)
            {
                float ySpeed = _body.linearVelocityY;

                float targetAngle = MathHelpers.Remap(
                    ySpeed,
                    -_maxFallSpeed,
                    _maxRiseSpeed,
                    _maxDownAngle,
                    _maxUpAngle
                );

                float currentZ = transform.eulerAngles.z;
                float smoothZ = Mathf.SmoothDampAngle(
                    currentZ,
                    targetAngle,
                    ref _rotationVelocity,
                    _rotationSmoothTime
                );

                transform.rotation = Quaternion.Euler(0f, 0f, smoothZ);
            }
        }

        private void Jump(float jumpForce)
        {
            _body.linearVelocityY = jumpForce;

            AdvancedSoundManager.Me.PlaySFX(_wingClips.RandomElement());
        }

        private void Die(float jumpForce)
        {
            AdvancedSoundManager.Me.PlaySFX(_hitClips.RandomElement());

            Jump(jumpForce);
            _state = BirdState.Falling;
            EventBus.Me.Publish(new OnBirdFall());
        }

        public void ResetState()
        {
            transform.position = _startPos;
            transform.localRotation = Quaternion.identity;
        }

        public void OnGameStarted(OnGameStarted args)
        {
            _body.simulated = true;
            _state = BirdState.Flying;
            MoveStarted = true;
        }

        public void OnGameReset(OnGameReset args)
        {
            ResetState();
            _body.simulated = false;
            _body.constraints = RigidbodyConstraints2D.None;
            MoveStarted = false;
            _state = BirdState.Selection;
            _waitTime = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Pipe"))
            {
                if (!IsFlying) return;

                Die(5);
            }
            else if (collision.gameObject.CompareTag("Point"))
            {
                if (!IsFlying) return;

                AdvancedSoundManager.Me.PlaySFX(_pointClips.RandomElement());

                GameManager.Me.AddPoint();
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                if (IsFalling)
                {
                    _body.simulated = false;
                    _body.constraints = RigidbodyConstraints2D.FreezeAll;
                    MoveStarted = false;

                    _state = BirdState.Died;
                    EventBus.Me.Publish(new OnGameEnd());
                }
                else
                {
                    Die(3.5f);
                }
            }
        }
    }
}