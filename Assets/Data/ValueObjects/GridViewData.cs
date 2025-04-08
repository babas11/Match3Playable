using System;
using UnityEngine;

namespace Data.ValueObjects
{
    [Serializable]
    public struct GridViewData
    {
        public Sprite GridBackground;
        public float BackgroundOverScale;
        public float GridUnit;
        public float GridBottomOffset;
    }
}