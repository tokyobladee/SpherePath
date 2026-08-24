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
        [SerializeField] private Color emptyEnergyColor = Color.red;
        [SerializeField] private Color halfEnergyColor = Color.yellow;
        [SerializeField] private Color fullEnergyColor = Color.green;

        private Rect _currentSafeArea;
        private Image _energyFillImage;

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
            var value = Mathf.Clamp01(normalizedEnergy);

            if (energySlider != null)
            {
                energySlider.value = value;
                UpdateEnergyFill(value);
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

        private void UpdateEnergyFill(float normalizedEnergy)
        {
            if (_energyFillImage == null && energySlider.fillRect != null)
            {
                _energyFillImage = energySlider.fillRect.GetComponent<Image>();
            }

            if (_energyFillImage == null)
            {
                return;
            }

            _energyFillImage.color = normalizedEnergy < 0.5f
                ? Color.Lerp(emptyEnergyColor, halfEnergyColor, normalizedEnergy * 2f)
                : Color.Lerp(halfEnergyColor, fullEnergyColor, (normalizedEnergy - 0.5f) * 2f);
        }
    }
}
