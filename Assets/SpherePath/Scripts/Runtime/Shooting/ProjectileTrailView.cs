using System.Collections.Generic;
using UnityEngine;

namespace SpherePath.Shooting
{
    public sealed class ProjectileTrailView : MonoBehaviour
    {
        private static readonly int UseRadialFadeProperty = Shader.PropertyToID("_UseRadialFade");

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private readonly List<Vector3> _positions = new List<Vector3>();
        private readonly List<float> _times = new List<float>();
        private Mesh _mesh;
        private float _lifeTime;
        private float _startWidth;
        private float _minVertexDistance;
        private MaterialPropertyBlock _propertyBlock;
        private bool _isConfigured;

        private void Awake()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponentInChildren<MeshFilter>();
            }

            if (meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<MeshRenderer>();
            }

            _mesh = new Mesh();
            _mesh.MarkDynamic();

            if (meshFilter != null)
            {
                meshFilter.sharedMesh = _mesh;
            }

            _propertyBlock = new MaterialPropertyBlock();
        }

        public void Configure(Material material, float lifeTime, float startWidth, float minVertexDistance)
        {
            _lifeTime = Mathf.Max(0.01f, lifeTime);
            _startWidth = Mathf.Max(0f, startWidth);
            _minVertexDistance = Mathf.Max(0f, minVertexDistance);
            _positions.Clear();
            _times.Clear();
            _isConfigured = meshFilter != null && meshRenderer != null;

            if (meshRenderer != null)
            {
                if (_propertyBlock == null)
                {
                    _propertyBlock = new MaterialPropertyBlock();
                }

                meshRenderer.sharedMaterial = material;
                meshRenderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(UseRadialFadeProperty, 0f);
                meshRenderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void LateUpdate()
        {
            if (!_isConfigured)
            {
                return;
            }

            AddCurrentPosition();
            RemoveExpiredPositions();
            RebuildMesh();
        }

        private void AddCurrentPosition()
        {
            var currentPosition = transform.position;

            if (_positions.Count == 0 || Vector3.Distance(_positions[_positions.Count - 1], currentPosition) >= _minVertexDistance)
            {
                _positions.Add(currentPosition);
                _times.Add(Time.time);
            }
        }

        private void RemoveExpiredPositions()
        {
            var minTime = Time.time - _lifeTime;

            while (_times.Count > 0 && _times[0] < minTime)
            {
                _times.RemoveAt(0);
                _positions.RemoveAt(0);
            }
        }

        private void RebuildMesh()
        {
            if (_positions.Count < 2)
            {
                _mesh.Clear();
                return;
            }

            var vertices = new Vector3[_positions.Count * 2];
            var triangles = new int[(_positions.Count - 1) * 12];
            var uvs = new Vector2[vertices.Length];
            var normals = new Vector3[vertices.Length];

            for (var i = 0; i < _positions.Count; i++)
            {
                var previous = i == 0 ? _positions[i] : _positions[i - 1];
                var next = i == _positions.Count - 1 ? _positions[i] : _positions[i + 1];
                var direction = (next - previous).normalized;
                var side = Vector3.Cross(Vector3.up, direction).normalized;
                var age = Mathf.Clamp01((Time.time - _times[i]) / _lifeTime);
                var width = Mathf.SmoothStep(_startWidth, 0f, age);
                var left = _positions[i] - side * width;
                var right = _positions[i] + side * width;
                vertices[i * 2] = meshFilter.transform.InverseTransformPoint(left);
                vertices[i * 2 + 1] = meshFilter.transform.InverseTransformPoint(right);
                uvs[i * 2] = new Vector2(age, 0f);
                uvs[i * 2 + 1] = new Vector2(age, 1f);
                normals[i * 2] = Vector3.up;
                normals[i * 2 + 1] = Vector3.up;
            }

            for (var i = 0; i < _positions.Count - 1; i++)
            {
                var vertexIndex = i * 2;
                var triangleIndex = i * 6;
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 2;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;
                triangles[triangleIndex + 6] = vertexIndex;
                triangles[triangleIndex + 7] = vertexIndex + 1;
                triangles[triangleIndex + 8] = vertexIndex + 2;
                triangles[triangleIndex + 9] = vertexIndex + 1;
                triangles[triangleIndex + 10] = vertexIndex + 3;
                triangles[triangleIndex + 11] = vertexIndex + 2;
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
