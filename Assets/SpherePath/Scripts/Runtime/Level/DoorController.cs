using UnityEngine;
using SpherePath.Configuration;

namespace SpherePath.Level
{
    public sealed class DoorController
    {
        private readonly GameplayConfiguration _configuration;
        private readonly Transform _leftPanel;
        private readonly Transform _rightPanel;

        private bool _isOpen;
        private float _openProgress;

        public DoorController(GameplayConfiguration configuration, LevelViewReferences scene)
        {
            _configuration = configuration;
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
            _openProgress = Mathf.MoveTowards(_openProgress, targetProgress, deltaTime * _configuration.DoorOpenSpeed);
            ApplyPose();
        }

        private void ApplyPose()
        {
            var easedProgress = Mathf.SmoothStep(0f, 1f, _openProgress);
            _leftPanel.localPosition = new Vector3(Mathf.Lerp(_configuration.DoorClosedLeftPanelX, _configuration.DoorOpenLeftPanelX, easedProgress), 0f, 0f);
            _rightPanel.localPosition = new Vector3(Mathf.Lerp(_configuration.DoorClosedRightPanelX, _configuration.DoorOpenRightPanelX, easedProgress), 0f, 0f);
        }
    }
}
