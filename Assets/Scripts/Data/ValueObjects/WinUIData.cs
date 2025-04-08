using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.ValueObjects
{
    [Serializable]
    public struct WinUIData
    {
        public GameObject ButtonPrefab;
        public Sprite VictoryBackGroundSprite;
        public Sprite StarSprite;
        public Sprite CompletedLabelSprite;
        public Sprite ContinueButtonSprite;
        public Sprite TransparentBackGroundSprite;
        public Vector3 VictoryBackPosition;
        public Vector3 VictoryBackScale;
        public Vector3 CompletedLabelPosition;
        public Vector3 CompletedLabelScale;
        public Vector3 WinStarPosition;
        public Vector3 WinStarScale;
        public Vector3 ContinueButtonPosition;
        public Vector3 ContinueLabelScale;
        public Vector3 TransparentBackgroundScale;
        public Color TransparentColor;
    }
}