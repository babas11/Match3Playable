using UnityEngine;

public class InteractablePool : ObjectPool<Interactable>
{
    [SerializeField]
    Sprite[] interactableSprites;

    [SerializeField]
    int howManyInteractables;

    SpriteMask spriteMask;

    private void Awake() {
        spriteMask = GetComponent<SpriteMask>();
        
    }


    public void InitializePool(Vector2Int gridSize)
    {
        CreatePool(gridSize.x * gridSize.y * 2);
    }

    public void SetInteractable(Interactable interactable)
    {
        int random = Random.Range(0, howManyInteractables);
        interactable.SetInteractable(interactableSprites[random], random);
    }

    public void SetInteractable(Interactable interactable,int id)
    {
        interactable.SetInteractable(interactableSprites[id], id);
    }

    public void ChangeInteractable(Interactable interactable){
        int interactableCurrentId = interactable.id;
        if(interactableCurrentId != howManyInteractables - 1){
            interactableCurrentId++;
        }else{
            interactableCurrentId = 0;
        }
        SetInteractable(interactable,interactableCurrentId);
    }
    
}