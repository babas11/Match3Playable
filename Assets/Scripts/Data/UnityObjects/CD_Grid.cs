using Data.ValueObjects;
using UnityEngine;

namespace Data.UnityObjects
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Match3/CD_Grid", order = 0)]
    public class CD_Grid : ScriptableObject
    {
        public GridViewData GridViewData;
        public GridSpinData GridSpinData;
    }
}