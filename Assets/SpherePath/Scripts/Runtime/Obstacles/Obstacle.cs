using System;
using System.Collections;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class Obstacle : MonoBehaviour
    {
        [SerializeField] private float radius = 0.5f;

        private float _clearFlashDuration;
        private float _clearShrinkDuration;
        private Vector3 _initialScale;
        private Renderer _renderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private Coroutine _clearRoutine;

        public event Action<Obstacle> Destroyed;

        public Vector3 Position => transform.position;

        public float Radius => radius;

        public bool IsCleared { get; private set; }

        public void Configure(float clearFlashDuration, float clearShrinkDuration)
        {
            _clearFlashDuration = clearFlashDuration;
            _clearShrinkDuration = clearShrinkDuration;
        }

        private void Awake()
        {
            _initialScale = transform.localScale;
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
            transform.localScale = _initialScale;
            ClearFlash();
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
                transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, normalized);
                yield return null;
            }

            transform.localScale = _initialScale;
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
            _materialPropertyBlock.SetColor("_BaseColor", Color.white);
            _materialPropertyBlock.SetColor("_Color", Color.white);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void ClearFlash()
        {
            if (_renderer == null)
            {
                return;
            }

            _renderer.SetPropertyBlock(null);
        }
    }
}
