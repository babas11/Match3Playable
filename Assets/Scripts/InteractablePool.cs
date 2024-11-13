using UnityEngine;

public class InteractablePool : ObjectPool<Interactable>
{
    [SerializeField]
    Sprite[] interactableSprites;

    [SerializeField]
    int howManyInteractables;

    public void InitializePool(Vector2Int gridSize)
    {
        CreatePool(gridSize.x * gridSize.y);
    }

    public void SetInteractable(Interactable interactable)
    {
        int random = Random.Range(0, howManyInteractables);
        interactable.SetInteractable(interactableSprites[random], random);
    }
    
}