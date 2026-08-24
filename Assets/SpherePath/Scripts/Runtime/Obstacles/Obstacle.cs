using System.Collections;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class Obstacle : MonoBehaviour
    {
        [SerializeField] private float radius = 0.5f;

        private Vector3 _initialScale;
        private Coroutine _clearRoutine;

        public Vector3 Position => transform.position;

        public float Radius => radius;

        public bool IsCleared { get; private set; }

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        public void Clear()
        {
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
            gameObject.SetActive(true);
        }

        private IEnumerator PlayClear()
        {
            var duration = 0.16f;
            var elapsed = 0f;
            var peakScale = _initialScale * 1.35f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var normalized = Mathf.Clamp01(elapsed / duration);
                transform.localScale = Vector3.Lerp(_initialScale, peakScale, Mathf.Sin(normalized * Mathf.PI));
                yield return null;
            }

            transform.localScale = _initialScale;
            gameObject.SetActive(false);
            _clearRoutine = null;
        }
    }
}
