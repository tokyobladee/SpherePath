using System.Collections.Generic;
using UnityEngine;

namespace SpherePath.Obstacles
{
    public sealed class ObstacleFieldLayout
    {
        private readonly Vector3[] _positions =
        {
            new Vector3(-2.2f, 0.6f, -4.8f),
            new Vector3(-0.8f, 0.6f, -4.4f),
            new Vector3(0.8f, 0.6f, -4.4f),
            new Vector3(2.2f, 0.6f, -4.8f),
            new Vector3(-1.7f, 0.6f, -2.4f),
            new Vector3(0f, 0.6f, -2.1f),
            new Vector3(1.7f, 0.6f, -2.4f),
            new Vector3(-2.4f, 0.6f, 0.1f),
            new Vector3(-0.7f, 0.6f, 0.4f),
            new Vector3(1f, 0.6f, 0.2f),
            new Vector3(2.5f, 0.6f, 0.9f),
            new Vector3(-1.9f, 0.6f, 2.9f),
            new Vector3(-0.2f, 0.6f, 3.2f),
            new Vector3(1.6f, 0.6f, 3f),
            new Vector3(-2.5f, 0.6f, 5.7f),
            new Vector3(-0.8f, 0.6f, 5.5f),
            new Vector3(0.9f, 0.6f, 5.8f),
            new Vector3(2.3f, 0.6f, 6.3f),
            new Vector3(-1.6f, 0.6f, 8.4f),
            new Vector3(0.1f, 0.6f, 8.8f),
            new Vector3(1.8f, 0.6f, 8.5f),
            new Vector3(-0.8f, 0.6f, 11.1f),
            new Vector3(0.9f, 0.6f, 11.3f)
        };

        public IReadOnlyList<Vector3> Positions => _positions;
    }
}
