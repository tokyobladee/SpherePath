using UnityEngine.InputSystem;

namespace SpherePath.Input
{
    public sealed class PointerChargeInput
    {
        private bool _wasPressed;

        public bool StartedThisFrame { get; private set; }

        public bool IsPressed { get; private set; }

        public bool ReleasedThisFrame { get; private set; }

        public bool RestartRequested { get; private set; }

        public void Tick()
        {
            var pointer = Pointer.current;
            var keyboard = Keyboard.current;
            var isPressed = pointer != null && pointer.press.isPressed;

            StartedThisFrame = isPressed && !_wasPressed;
            ReleasedThisFrame = !isPressed && _wasPressed;
            IsPressed = isPressed;
            RestartRequested = keyboard != null && keyboard.rKey.wasPressedThisFrame;
            _wasPressed = isPressed;
        }
    }
}
