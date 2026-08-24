using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpherePath.UI
{
    public sealed class GameUiView
    {
        private readonly Slider _energySlider;
        private readonly Text _statusText;
        private readonly GameObject _resultPanel;
        private readonly Text _resultHintText;
        private readonly Button _restartButton;
        private readonly RectTransform _safeArea;

        private Rect _currentSafeArea;

        public GameUiView(Slider energySlider, Text statusText, GameObject resultPanel, Text resultHintText, Button restartButton, RectTransform safeArea)
        {
            _energySlider = energySlider;
            _statusText = statusText;
            _resultPanel = resultPanel;
            _resultHintText = resultHintText;
            _restartButton = restartButton;
            _safeArea = safeArea;
            _restartButton.onClick.AddListener(HandleRestartClicked);
            RefreshSafeArea();
        }

        public event Action RestartClicked;

        public void Dispose()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(HandleRestartClicked);
            }
        }

        public void SetEnergy(float normalizedEnergy)
        {
            RefreshSafeArea();

            if (_energySlider != null)
            {
                _energySlider.value = normalizedEnergy;
            }
        }

        public void ShowPlaying()
        {
            SetStatus(string.Empty);

            if (_resultPanel != null)
            {
                _resultPanel.SetActive(false);
            }
        }

        public void ShowResult(string title, string hint)
        {
            SetStatus(title);

            if (_resultHintText != null)
            {
                _resultHintText.text = hint;
            }

            if (_resultPanel != null)
            {
                _resultPanel.SetActive(true);
            }
        }

        private void SetStatus(string value)
        {
            if (_statusText != null)
            {
                _statusText.text = value;
            }
        }

        private void HandleRestartClicked()
        {
            RestartClicked?.Invoke();
        }

        private void RefreshSafeArea()
        {
            if (_safeArea == null || _currentSafeArea == Screen.safeArea || Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }

            _currentSafeArea = Screen.safeArea;
            var anchorMin = _currentSafeArea.position;
            var anchorMax = _currentSafeArea.position + _currentSafeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            _safeArea.anchorMin = anchorMin;
            _safeArea.anchorMax = anchorMax;
            _safeArea.offsetMin = Vector2.zero;
            _safeArea.offsetMax = Vector2.zero;
        }
    }
}
