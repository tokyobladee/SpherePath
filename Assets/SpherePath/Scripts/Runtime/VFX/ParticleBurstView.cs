using UnityEngine;

namespace SpherePath.VFX
{
    public sealed class ParticleBurstView : MonoBehaviour
    {
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int UseRadialFadeProperty = Shader.PropertyToID("_UseRadialFade");

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private int particleSides = 10;

        private MaterialPropertyBlock _propertyBlock;
        private BurstParticle[] _particles;
        private Mesh _mesh;
        private float _elapsedTime;
        private float _objectLifetime;
        private float _gravity;
        private bool _isPlaying;

        private struct BurstParticle
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public float Size;
            public float Lifetime;
            public float Age;
        }

        private void Awake()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            _mesh = new Mesh();
            _mesh.MarkDynamic();

            if (meshFilter != null)
            {
                meshFilter.sharedMesh = _mesh;
            }

            _propertyBlock = new MaterialPropertyBlock();
        }

        public void Play(Vector3 position, Material material, Color color, float particleLifetime, float speed, float size, float gravity, int maxParticles, float shapeRadius, int particleCount, float objectLifetime)
        {
            transform.position = position;
            _elapsedTime = 0f;
            _objectLifetime = Mathf.Max(0.01f, objectLifetime);
            _gravity = gravity;
            var count = Mathf.Clamp(particleCount, 0, maxParticles);
            _particles = new BurstParticle[count];
            _isPlaying = meshFilter != null && meshRenderer != null;

            if (meshRenderer != null)
            {
                if (_propertyBlock == null)
                {
                    _propertyBlock = new MaterialPropertyBlock();
                }

                meshRenderer.sharedMaterial = material;
                meshRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(BaseColorProperty, color);
                _propertyBlock.SetColor(ColorProperty, color);
                _propertyBlock.SetFloat(UseRadialFadeProperty, 1f);
                meshRenderer.SetPropertyBlock(_propertyBlock);
            }

            for (var i = 0; i < count; i++)
            {
                var randomDirection = Random.insideUnitCircle;
                var direction = randomDirection.sqrMagnitude <= Mathf.Epsilon
                    ? Vector3.forward
                    : new Vector3(randomDirection.x, 0f, randomDirection.y).normalized;
                var offset = direction * Random.Range(0f, shapeRadius);
                _particles[i] = new BurstParticle
                {
                    Position = offset,
                    Velocity = direction * speed,
                    Size = size,
                    Lifetime = Mathf.Max(0.01f, particleLifetime),
                    Age = 0f
                };
            }

            RebuildMesh();
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _objectLifetime)
            {
                Destroy(gameObject);
                return;
            }

            for (var i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                particle.Age += Time.deltaTime;
                particle.Velocity += Vector3.down * _gravity * Time.deltaTime;
                particle.Position += particle.Velocity * Time.deltaTime;
                _particles[i] = particle;
            }

            RebuildMesh();
        }

        private void RebuildMesh()
        {
            if (_particles == null || _particles.Length == 0)
            {
                _mesh.Clear();
                return;
            }

            var sideCount = Mathf.Max(3, particleSides);
            var verticesPerParticle = sideCount + 1;
            var trianglesPerParticle = sideCount * 6;
            var vertices = new Vector3[_particles.Length * verticesPerParticle];
            var triangles = new int[_particles.Length * trianglesPerParticle];
            var uvs = new Vector2[vertices.Length];
            var normals = new Vector3[vertices.Length];

            for (var i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                var age = Mathf.Clamp01(particle.Age / particle.Lifetime);
                var particleSize = Mathf.SmoothStep(particle.Size, 0f, age);
                var vertexIndex = i * verticesPerParticle;
                var triangleIndex = i * trianglesPerParticle;
                vertices[vertexIndex] = particle.Position;
                uvs[vertexIndex] = new Vector2(0.5f, 0.5f);
                normals[vertexIndex] = Vector3.up;

                for (var side = 0; side < sideCount; side++)
                {
                    var angle = Mathf.PI * 2f * side / sideCount;
                    var direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                    vertices[vertexIndex + side + 1] = particle.Position + direction * particleSize;
                    uvs[vertexIndex + side + 1] = new Vector2((direction.x + 1f) * 0.5f, (direction.z + 1f) * 0.5f);
                    normals[vertexIndex + side + 1] = Vector3.up;
                    triangles[triangleIndex + side * 6] = vertexIndex;
                    triangles[triangleIndex + side * 6 + 1] = vertexIndex + (side + 1) % sideCount + 1;
                    triangles[triangleIndex + side * 6 + 2] = vertexIndex + side + 1;
                    triangles[triangleIndex + side * 6 + 3] = vertexIndex;
                    triangles[triangleIndex + side * 6 + 4] = vertexIndex + side + 1;
                    triangles[triangleIndex + side * 6 + 5] = vertexIndex + (side + 1) % sideCount + 1;
                }
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.uv = uvs;
            _mesh.normals = normals;
            _mesh.RecalculateBounds();
        }
    }
}
