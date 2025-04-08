using Data.ValueObjects;
using UnityEngine;

namespace Data.UnityObjects
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "Match3/CD_UI", order = 0)]
    public class CD_UI : ScriptableObject
    {
        public LevelUIData LevelUIData;
        public WinUIData WinUIData;
    }
}