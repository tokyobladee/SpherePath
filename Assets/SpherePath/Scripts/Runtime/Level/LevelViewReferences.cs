using System.Collections.Generic;
using SpherePath.Cameras;
using SpherePath.Obstacles;
using SpherePath.Player;
using SpherePath.UI;
using UnityEngine;

namespace SpherePath.Level
{
    public sealed class LevelViewReferences
    {
        public LevelViewReferences(
            IReadOnlyList<Obstacle> obstacles,
            PlayerView player,
            Transform door,
            Transform doorLeftPanel,
            Transform doorRightPanel,
            Transform corridor,
            Transform chargePreview,
            FollowCameraView cameraView,
            GameUiView ui)
        {
            Obstacles = obstacles;
            Player = player;
            Door = door;
            DoorLeftPanel = doorLeftPanel;
            DoorRightPanel = doorRightPanel;
            Corridor = corridor;
            ChargePreview = chargePreview;
            CameraView = cameraView;
            Ui = ui;
        }

        public IReadOnlyList<Obstacle> Obstacles { get; }

        public PlayerView Player { get; }

        public Transform Door { get; }

        public Transform DoorLeftPanel { get; }

        public Transform DoorRightPanel { get; }

        public Transform Corridor { get; }

        public Transform ChargePreview { get; }

        public FollowCameraView CameraView { get; }

        public GameUiView Ui { get; }
    }
}
