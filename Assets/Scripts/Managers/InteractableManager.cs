using DG.Tweening;
using Enums;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractableManager : MonoBehaviour
    {
        [SerializeField]
        public int id; 

        public Vector2Int matrixPosition = new Vector2Int(-1, -1);
        SpriteRenderer spriteRenderer;

        public bool Idle = true;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            matrixPosition = new Vector2Int(-1, -1);
        }

        public void SetInteractableInMatrix(Sprite sprite, int id)
        {
            spriteRenderer.sprite = sprite;
            this.id = id;
        }

        private void OnMouseDown()
        {
            if (GameManager.State == GameStates.SpinActive)
                GameManager.Instance.UpdateGameState(GameStates.MatchingActive);

            if (GameManager.State == GameStates.MatchingActive)
            {
                transform.DOScale(1.1f, 0.1f).OnComplete(() =>
                {
                    transform.DOScale(1f, 0.1f);
                });

                SelectorManager.instance.SelectFirst(this);
                print($"selected {this.matrixPosition}");
            }
        }
        private void OnMouseUp()
        {
            if (GameManager.State == GameStates.MatchingActive)
            {
                SelectorManager.instance.SelectFirst(null);
            }
        }

        private void OnMouseEnter()
        {
            if (GameManager.State == GameStates.MatchingActive)
            {
                //print("Mouse enetered at (" + matrixPosition.x + "," + matrixPosition.y + ")");
                SelectorManager.instance.SelectSecond(this);
            }

        }
        public override string ToString()
        {
            return $"interactable id: {id}, matrix position: {matrixPosition}";
        }




    }
}
