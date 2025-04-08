using Data.UnityObjects;
using UnityEngine;

namespace Controllers.Grid
{
    public class GridSpriteController : MonoBehaviour
    {
        #region Self Variables
        
        #region Serialized Variables

        [SerializeField] private SpriteRenderer _renderer;

        #endregion
        
        #region Private Variables

        private CD_Grid _data;
        private Vector2Int _dimensions;
        private Vector3 _initialPosition;
        
        #endregion
        
        #endregion
        
        public void SetRendererData(Vector2Int dimensions,CD_Grid data = null)
        {
            if( data != null)
                _data = data;
            _dimensions = dimensions;
            _initialPosition = transform.position;
            
        }
        
        
        public void SetGridBackGroundSprite()
        {
            _renderer.sprite = _data.GridViewData.GridBackground;
            float overScale = _data.GridViewData.BackgroundOverScale;
            float gridUnit = _data.GridViewData.GridUnit;

            transform.localPosition = Vector3.zero;
            
            float widthCenter = transform.position.x + _dimensions.x * gridUnit / 2f - gridUnit/2f;
            float heightCenter = transform.position.y + _dimensions.y * gridUnit / 2f - gridUnit/2f ;
            
            Vector3 center = new Vector3(widthCenter, heightCenter, 0);
            transform.position = center;
            
            _renderer.size = new Vector2(_dimensions.x * gridUnit * overScale , _dimensions.y * overScale * gridUnit  );
        }
    }
}