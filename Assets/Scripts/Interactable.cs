using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour
{
    public int id { get; private set; }
    public Vector2Int matrixPosition { get;set; }
    SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetInteractable(Sprite sprite, int id)
    {
        spriteRenderer.sprite = sprite;
        this.id = id;
    }
  
    
}