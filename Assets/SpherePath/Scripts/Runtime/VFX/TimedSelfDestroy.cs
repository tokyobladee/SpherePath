using UnityEngine;

namespace SpherePath.VFX
{
    public sealed class TimedSelfDestroy : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 0.3f;

        public void SetLifeTime(float value)
        {
            lifeTime = Mathf.Max(0.01f, value);
        }

        private void Update()
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
