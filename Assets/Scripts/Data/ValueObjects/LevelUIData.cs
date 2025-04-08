using System;
using UnityEngine;

namespace Data.ValueObjects
{
    [Serializable]
    public struct LevelUIData
    {
        public GameObject ButtonPrefab;
        public float SpinButtonDistance;
        public Vector3 SpinButtonScale;
        public Vector3 SpinButtonMaxAnimScale;
        public Sprite ButtonStartImage;
        public Sprite ButtonStopImage;
        public float ButtonOffScreenDistance;
    }
}