using Data.UnityObjects;
using UnityEngine;

namespace Controllers.Grid
{
    public class GridMaskController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private SpriteMask _spriteMask;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private float maskOverSize = 1.5f;

        #endregion

        #region Private Variables

        private CD_Grid _data;
        private Vector2Int _dimensions;

        #endregion

        #endregion

        public void SetMaskControllerData(CD_Grid data, Vector2Int dimensions)
        {
            _data = data;
            _dimensions = dimensions;
        }

        public void SetSpriteMask()
        {
            _spriteMask.sprite = _renderer.sprite;
            _spriteMask.alphaCutoff = 0;
            _spriteMask.transform.localScale = new Vector3(_dimensions.x +maskOverSize, _dimensions.y +maskOverSize);
        }
    }
}