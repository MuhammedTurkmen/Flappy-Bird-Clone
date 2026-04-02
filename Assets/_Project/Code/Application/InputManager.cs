using MET.Core.Patterns;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MET.Core.Manager
{
    public class InputManager : Singleton<InputManager>
    {
        public bool JumpPressed => !_pointerIsOverUI && _jumpAction != null && _jumpAction.WasPressedThisFrame();

        private bool _pointerIsOverUI => EventSystem.current.IsPointerOverGameObject();

        // INPUT MAP
        private InputActionMap _gameplayMap;

        // INPUT ACTION
        private InputAction _jumpAction;

        protected override void Awake()
        {
            base.Awake();

            _gameplayMap = new InputActionMap("Gameplay");

            _jumpAction = _gameplayMap.AddAction("Jump", binding: null, type: InputActionType.Button);

#if UNITY_ANDROID
            _jumpAction.AddBinding("<Touchscreen>/primaryTouch/press");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            _jumpAction.AddBinding("<Keyboard>/space");
            _jumpAction.AddBinding("<Mouse>/leftButton");
#endif

            _gameplayMap.Enable();
        }

        private void OnDestroy()
        {
            if (_gameplayMap != null)
            {
                _gameplayMap.Disable();
                _gameplayMap.Dispose();
            }
        }
    }
}
