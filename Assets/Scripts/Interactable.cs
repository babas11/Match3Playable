using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    public int id; //{ get; private set; }

    
    public Vector2Int matrixPosition = new Vector2Int(-1,-1); //{ get;set; }
    SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        matrixPosition = new Vector2Int(-1,-1);
    }

    public void SetInteractable(Sprite sprite, int id)
    {
        spriteRenderer.sprite = sprite;
        this.id = id;
    }
  
    
}