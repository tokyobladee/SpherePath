using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpherePath.UI
{
    public sealed class GameUiView : MonoBehaviour
    {
        [SerializeField] private Slider energySlider;
        [SerializeField] private Text statusText;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text resultHintText;
        [SerializeField] private Button restartButton;
        [SerializeField] private RectTransform safeArea;
        [SerializeField] private Image levelProgressFill;
        [SerializeField] private Text currentLevelText;
        [SerializeField] private Text nextLevelText;

        private Rect _currentSafeArea;

        public event Action RestartClicked;

        private void OnEnable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(HandleRestartClicked);
            }

            RefreshSafeArea();
        }

        private void OnDisable()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
            }
        }

        public void SetLevelProgress(int currentLevel, string nextLevel, float normalizedProgress)
        {
            if (currentLevelText != null)
            {
                currentLevelText.text = currentLevel.ToString();
            }

            if (nextLevelText != null)
            {
                nextLevelText.text = nextLevel;
            }

            if (levelProgressFill != null)
            {
                levelProgressFill.fillAmount = Mathf.Clamp01(normalizedProgress);
            }
        }

        public void SetLevelProgressValue(float normalizedProgress)
        {
            if (levelProgressFill != null)
            {
                levelProgressFill.fillAmount = Mathf.Clamp01(normalizedProgress);
            }
        }

        public void SetEnergy(float normalizedEnergy)
        {
            RefreshSafeArea();

            if (energySlider != null)
            {
                energySlider.value = normalizedEnergy;
            }
        }

        public void ShowPlaying()
        {
            SetStatus(string.Empty);

            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }

        public void ShowResult(string title, string hint)
        {
            SetStatus(title);

            if (resultHintText != null)
            {
                resultHintText.text = hint;
            }

            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
            {
                statusText.text = value;
            }
        }

        private void HandleRestartClicked()
        {
            RestartClicked?.Invoke();
        }

        private void RefreshSafeArea()
        {
            if (safeArea == null || _currentSafeArea == Screen.safeArea || Screen.width <= 0 || Screen.height <= 0)
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
            safeArea.anchorMin = anchorMin;
            safeArea.anchorMax = anchorMax;
            safeArea.offsetMin = Vector2.zero;
            safeArea.offsetMax = Vector2.zero;
        }
    }
}
