using System.Collections.Generic;
using SpherePath.Cameras;
using SpherePath.Obstacles;
using SpherePath.Player;
using SpherePath.UI;
using UnityEngine;

namespace SpherePath.Level
{
    public sealed class LevelViewReferences : MonoBehaviour
    {
        [SerializeField] private List<Obstacle> obstacles = new List<Obstacle>();
        [SerializeField] private PlayerView player;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform door;
        [SerializeField] private Transform doorLeftPanel;
        [SerializeField] private Transform doorRightPanel;
        [SerializeField] private Transform corridor;
        [SerializeField] private Transform chargePreview;
        [SerializeField] private FollowCameraView cameraView;
        [SerializeField] private GameUiView ui;
        [SerializeField] private Material projectileMaterial;
        [SerializeField] private Material infectionPreviewMaterial;
        [SerializeField] private Material trailMaterial;

        public IReadOnlyList<Obstacle> Obstacles => obstacles;

        public PlayerView Player => player;

        public Vector3 PlayerSpawnPosition => playerSpawnPoint.position;

        public Vector3 DoorPosition => door.position;

        public Transform DoorLeftPanel => doorLeftPanel;

        public Transform DoorRightPanel => doorRightPanel;

        public Transform Corridor => corridor;

        public Transform ChargePreview => chargePreview;

        public FollowCameraView CameraView => cameraView;

        public GameUiView Ui => ui;

        public Material ProjectileMaterial => projectileMaterial;

        public Material InfectionPreviewMaterial => infectionPreviewMaterial;

        public Material TrailMaterial => trailMaterial;

        public void Validate()
        {
            if (player == null || playerSpawnPoint == null || door == null || doorLeftPanel == null || doorRightPanel == null || corridor == null || chargePreview == null || cameraView == null || ui == null)
            {
                throw new System.InvalidOperationException($"{nameof(LevelViewReferences)} has missing required references.");
            }

            if (projectileMaterial == null || infectionPreviewMaterial == null || trailMaterial == null)
            {
                throw new System.InvalidOperationException($"{nameof(LevelViewReferences)} has missing transient materials.");
            }
        }
    }
}
