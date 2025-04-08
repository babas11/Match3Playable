using Data.UnityObjects;
using UnityEngine;

namespace Commands.Grid
{
    public class PlaceGridCommand
    {
        private CD_Grid _data;
        private Vector2Int _dimensions;
        private Transform _gridTransform;
        
        float GridWidth => _dimensions.x * _data.GridViewData.GridUnit;
        float GridHeight => _dimensions.y * _data.GridViewData.GridUnit;
        
        public PlaceGridCommand(CD_Grid data,Transform gridTransform, Vector2Int dimensions)
        {
            _data = data;
            _gridTransform = gridTransform;
            _dimensions = dimensions;
        }

        internal void Execute()
        {
            float gridUnit = _data.GridViewData.GridUnit;
            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));

            float worldWidth = topRight.x - bottomLeft.x;

            //Add half of the grid element size
            float halfGridUnit = gridUnit / 2f;

            // Center the grid horizontally and apply vertical offset
            float gridXPosition = bottomLeft.x + (worldWidth - GridWidth)/ 2f + halfGridUnit;
            float gridYPosition = bottomLeft.y + _data.GridViewData.GridBottomOffset + halfGridUnit;

            // Set the position of the grid
            _gridTransform.position = new Vector3(gridXPosition, gridYPosition, 0f);
        }
    }
}