using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class PrototypeObstacle : MonoBehaviour
    {
        [SerializeField] private float radius = 0.5f;

        public Vector3 Position => transform.position;

        public float Radius => radius;

        public bool IsCleared { get; private set; }

        public void Clear()
        {
            IsCleared = true;
            gameObject.SetActive(false);
        }

        public void Restore()
        {
            IsCleared = false;
            gameObject.SetActive(true);
        }
    }
}
