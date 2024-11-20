using UnityEngine;

public class InteractablePool : ObjectPool<Interactable>
{
    [SerializeField]
    Sprite[] interactableSprites;

    [SerializeField]
    int howManyInteractables;

    SpriteMask spriteMask;

    public bool Ready {get;private set;} = false;

    private void Awake()
    {
        spriteMask = GetComponent<SpriteMask>();

    }


    public void InitializePool(Vector2Int gridSize)
    {
        CreatePool(gridSize.x * gridSize.y * 2);
        Ready = true;
    }

    public void SetInteractable(Interactable interactable)
    {
        int random = Random.Range(0, howManyInteractables);
        interactable.SetInteractableInMatrix(interactableSprites[random], random);
    }

    public void SetInteractable(Interactable interactable, int id)
    {
        interactable.SetInteractableInMatrix(interactableSprites[id], id);
    }
    public void SetToNextInteractable(Interactable interactable)
    {
        int idToMove = interactable.id;

        if (idToMove == howManyInteractables - 1)
        {
            SetInteractable(interactable, 0);
        }
        else
        {
            SetInteractable(interactable, idToMove++);
        }
    }

    public void ChangeInteractable(Interactable interactable)
    {
        int interactableCurrentId = interactable.id;
        if (interactableCurrentId != howManyInteractables - 1)
        {
            interactableCurrentId++;
        }
        else
        {
            interactableCurrentId = 0;
        }
        SetInteractable(interactable, interactableCurrentId);
    }

    public void ClearInteractablePool()
    {
        Vector2Int defaultMatrixPosition = new Vector2Int(-1, -1);
        foreach (Interactable element in pool)
        {
            element.matrixPosition = defaultMatrixPosition;
            element.id = default;
        }
        ClearPool();

    }

}