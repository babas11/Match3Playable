using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractablePool : ObjectPool<Interactable>
{
    [SerializeField] Sprite[] interactableSprites;

    [SerializeField] int howManyInteractables;

    SpriteMask spriteMask;

    public bool Ready { get; private set; } = false;

    int minimumAmountOfEachType = 3;

    private void Awake()
    {
        spriteMask = GetComponent<SpriteMask>();
    }


    public void InitializePoolForGrid(Vector2Int gridSize)
    {
        CreatePool(gridSize.x * gridSize.y);
        Ready = true;
    }

    public void AssignRandomTypeToInteractable(Interactable interactable)
    {
        int random = UnityEngine.Random.Range(0, howManyInteractables);
        interactable.SetInteractableInMatrix(interactableSprites[random], random);
    }

    public void AssignRandomTypeToInteractable(List<Interactable> interactables, Interactable interactable)
    {
        int random = UnityEngine.Random.Range(0, howManyInteractables);
        interactable.SetInteractableInMatrix(interactableSprites[random], random);
    }

    public void AssignTypeToInteractable(Interactable interactable, int id)
    {
        interactable.SetInteractableInMatrix(interactableSprites[id], id);
    }

    public void CycleInteractableType(Interactable interactable)
    {
        int currentTypeId = interactable.id;

        if (currentTypeId == howManyInteractables - 1)
        {
            AssignTypeToInteractable(interactable, 0);
        }
        else
        {
            AssignTypeToInteractable(interactable, currentTypeId + 1);
        }
    }

    public List<Interactable> GenerateMinimumTypeInteractables(int minPerType)
    {
        if (minPerType <= 0)
        {
            Debug.LogError("Minimum amount of each type shold be greater than zero");
            return null;
        }

        List<Interactable> interactables = new List<Interactable>(minPerType * howManyInteractables);
        for (int typeId = 0; typeId < howManyInteractables; typeId++)
        {
            for (int count = 0; count < minPerType; count++)
            {
                Interactable newInteractable = GetObjectFromPool();
                AssignTypeToInteractable(newInteractable, typeId);
                interactables.Add(newInteractable);
            }
        }

        return interactables;
    }

    public List<Interactable> CreateInteractablesWithMinimumTypesAndRandomFill(int minEachType, int totalAmount)
    {
        if (minEachType <= 0 || totalAmount <= 0 || totalAmount < minEachType * howManyInteractables)
        {
            Debug.LogError($"Invalid Input");
            return null;
        }

        if (totalAmount < minEachType * howManyInteractables)
        {
            Debug.LogError($"Total amount should be greater to have {minEachType} of all types.");
            return null;
        }

        List<Interactable> interactables = new List<Interactable>(totalAmount);
        interactables.AddRange(GenerateMinimumTypeInteractables(minEachType));


        for (int i = 0; i < totalAmount - minEachType * howManyInteractables; i++)
        {
            Interactable newInteractable = GetObjectFromPool();
            AssignRandomTypeToInteractable(newInteractable);
            interactables.Add(newInteractable);
        }

        ShuffleWithSeed(interactables, totalAmount);
        return ArrangeInteractablesWithNoSameEachOther(interactables);
    }

    void ShuffleWithSeed<T>(IList<T> list, int seed)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    List<Interactable> ArrangeInteractablesWithNoSameEachOther(List<Interactable> interactables)
    {
        for (int i = 0; i < interactables.Count - 1; i++)
        {
            if (interactables[i].id == interactables[i + 1].id)
            {
                bool isChanged = false;
                for (int j = i + 2; j < interactables.Count; j++)
                {
                    if (interactables[j].id != interactables[i].id)
                    {
                        SwapInteractables(interactables, i + 1, j);
                        isChanged = true;
                        break;
                    }
                }

                if (!isChanged)
                {
                    // Since we're at the end of the list, swap with a previous element
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (interactables[j].id != interactables[i].id)
                        {
                            SwapInteractables(interactables, i + 1, j);
                            isChanged = true;
                            break;
                        }
                    }

                    // If still not changed, assign a new random type
                    if (!isChanged)
                    {
                        AssignRandomTypeToInteractable(interactables[i + 1]);
                    }
                }
            }
        }

        return interactables;
    }

    private void SwapInteractables(List<Interactable> interactables, int firstInteractable, int secondInteractable)
    {
        Interactable temp = interactables[firstInteractable];
        interactables[firstInteractable] = interactables[secondInteractable];
        interactables[secondInteractable] = temp;
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

        AssignTypeToInteractable(interactable, interactableCurrentId);
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