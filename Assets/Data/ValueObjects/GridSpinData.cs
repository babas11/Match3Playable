using System;
using UnityEngine;

namespace Data.ValueObjects
{
    [Serializable]
    public struct GridSpinData
    {
        public float spinSpeedBottomLimit ;
        public float deceleration;
        public int minimumAmountOfEachType;
    }
}