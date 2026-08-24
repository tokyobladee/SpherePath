using System;
using System.Collections;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class Obstacle : MonoBehaviour
    {
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly Vector3 VariationHashDirection = new Vector3(12.9898f, 78.233f, 37.719f);
        private const float VariationHashMultiplier = 43758.5453f;
        private const float HeightVariationSeedOffset = 1.37f;

        [SerializeField] private float radius = 0.5f;

        private float _clearFlashDuration;
        private float _clearShrinkDuration;
        private Vector3 _baseScale;
        private Vector3 _visualScale;
        private Color _visualColor;
        private Renderer _renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private Coroutine _clearRoutine;

        public event Action<Obstacle> Destroyed;

        public Vector3 Position => transform.position;

        public float Radius => radius;

        public bool IsCleared { get; private set; }

        public void Configure(float clearFlashDuration, float clearShrinkDuration, Color baseColor, Color accentColor, float colorVariation, float heightVariation)
        {
            _clearFlashDuration = clearFlashDuration;
            _clearShrinkDuration = clearShrinkDuration;
            ApplyVariation(baseColor, accentColor, colorVariation, heightVariation);
        }

        private void Awake()
        {
            _baseScale = transform.localScale;
            _visualScale = _baseScale;
            _renderer = GetComponent<Renderer>();
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void Clear()
        {
            if (IsCleared)
            {
                return;
            }

            IsCleared = true;

            if (_clearRoutine != null)
            {
                StopCoroutine(_clearRoutine);
            }

            _clearRoutine = StartCoroutine(PlayClear());
        }

        public void Restore()
        {
            if (_clearRoutine != null)
            {
                StopCoroutine(_clearRoutine);
                _clearRoutine = null;
            }

            IsCleared = false;
            transform.localScale = _visualScale;
            ApplyColor(_visualColor);
            gameObject.SetActive(true);
        }

        private IEnumerator PlayClear()
        {
            ApplyFlash();
            yield return new WaitForSeconds(Mathf.Max(0f, _clearFlashDuration));

            var elapsed = 0f;

            while (elapsed < _clearShrinkDuration)
            {
                elapsed += Time.deltaTime;
                var normalized = Mathf.Clamp01(elapsed / _clearShrinkDuration);
                transform.localScale = Vector3.Lerp(_visualScale, Vector3.zero, normalized);
                yield return null;
            }

            transform.localScale = _visualScale;
            Destroyed?.Invoke(this);
            gameObject.SetActive(false);
            _clearRoutine = null;
        }

        private void ApplyFlash()
        {
            if (_renderer == null)
            {
                return;
            }

            _renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(BaseColorProperty, Color.white);
            _materialPropertyBlock.SetColor(ColorProperty, Color.white);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void ApplyVariation(Color baseColor, Color accentColor, float colorVariation, float heightVariation)
        {
            var seed = Mathf.Sin(Vector3.Dot(transform.position, VariationHashDirection)) * VariationHashMultiplier;
            var normalized = Mathf.Repeat(seed, 1f);
            var colorBlend = Mathf.Lerp(0f, normalized, Mathf.Clamp01(colorVariation));
            var heightScale = 1f + Mathf.Lerp(-heightVariation, heightVariation, Mathf.Repeat(seed * HeightVariationSeedOffset, 1f));
            _visualColor = Color.Lerp(baseColor, accentColor, colorBlend);
            _visualScale = new Vector3(_baseScale.x, _baseScale.y * heightScale, _baseScale.z);
            transform.localScale = _visualScale;
            ApplyColor(_visualColor);
        }

        private void ApplyColor(Color color)
        {
            if (_renderer == null)
            {
                return;
            }

            _renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(BaseColorProperty, color);
            _materialPropertyBlock.SetColor(ColorProperty, color);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}
