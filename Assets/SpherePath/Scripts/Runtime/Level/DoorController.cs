using UnityEngine;

namespace SpherePath.Level
{
    public sealed class DoorController
    {
        private const float ClosedLeftPanelX = -0.45f;
        private const float OpenLeftPanelX = -0.9f;
        private const float ClosedRightPanelX = 0.45f;
        private const float OpenRightPanelX = 0.9f;
        private const float OpenSpeed = 4f;

        private readonly Transform _leftPanel;
        private readonly Transform _rightPanel;

        private bool _isOpen;
        private float _openProgress;

        public DoorController(LevelViewReferences scene)
        {
            _leftPanel = scene.DoorLeftPanel;
            _rightPanel = scene.DoorRightPanel;
        }

        public void Reset()
        {
            _isOpen = false;
            _openProgress = 0f;
            ApplyPose();
        }

        public void SetOpen(bool isOpen)
        {
            _isOpen = isOpen;
        }

        public void Tick(float deltaTime)
        {
            var targetProgress = _isOpen ? 1f : 0f;
            _openProgress = Mathf.MoveTowards(_openProgress, targetProgress, deltaTime * OpenSpeed);
            ApplyPose();
        }

        private void ApplyPose()
        {
            var easedProgress = Mathf.SmoothStep(0f, 1f, _openProgress);
            _leftPanel.localPosition = new Vector3(Mathf.Lerp(ClosedLeftPanelX, OpenLeftPanelX, easedProgress), 0f, 0f);
            _rightPanel.localPosition = new Vector3(Mathf.Lerp(ClosedRightPanelX, OpenRightPanelX, easedProgress), 0f, 0f);
        }
    }
}
