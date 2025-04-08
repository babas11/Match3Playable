using System;
using System.Collections.Generic;
using Signals;
using UnityEngine;

namespace Managers
{
    public class PoolManager : ObjectPool<InteractableManager>
    {
    
        [SerializeField] Sprite[] interactableSprites;
        [SerializeField] int howManyInteractables;
        
        public bool Ready { get; private set; } = false;

        private int minimumAmountOfEachType = 3;
        
        void OnEnable()
        {
            GameSignals.Instance.onGridInitialize += InitializePoolForGrid;
            GameSignals.Instance.onGetStartInteractables += CreateInteractablesWithMinimumTypesAndRandomFill;
            GameSignals.Instance.onGetInteractable += GetObjectFromPool;
            GameSignals.Instance.onCycleInteractableType += CycleInteractableType;
            GameSignals.Instance.onAssignRandomTypeToInteractable += AssignRandomTypeToInteractable;
            GameSignals.Instance.onAssignTypeToInteractable += AssignTypeToInteractable;
            GameSignals.Instance.onObjectReturnToPool += ReturnObjectToPool;
            GameSignals.Instance.onClearPool += ClearPool;

        }

        private void OnDisable()
        { 
            GameSignals.Instance.onGridInitialize -= InitializePoolForGrid;
            GameSignals.Instance.onGetStartInteractables -= CreateInteractablesWithMinimumTypesAndRandomFill;
            GameSignals.Instance.onGetInteractable -= GetObjectFromPool;
            GameSignals.Instance.onCycleInteractableType -= CycleInteractableType;
            GameSignals.Instance.onAssignRandomTypeToInteractable -= AssignRandomTypeToInteractable;
            GameSignals.Instance.onAssignTypeToInteractable -= AssignTypeToInteractable;
            GameSignals.Instance.onObjectReturnToPool += ReturnObjectToPool;
            GameSignals.Instance.onClearPool -= ClearPool;
        }


        public void InitializePoolForGrid(Vector2Int gridSize)
        {
            CreatePool(gridSize.x * gridSize.y);
            Ready = true;
        }

        public void AssignRandomTypeToInteractable(InteractableManager interactableManager)
        {
            int random = UnityEngine.Random.Range(0, howManyInteractables);
            interactableManager.SetInteractableInMatrix(interactableSprites[random], random);
        }

        public void AssignRandomTypeToInteractable(List<InteractableManager> interactables, InteractableManager interactableManager)
        {
            int random = UnityEngine.Random.Range(0, howManyInteractables);
            interactableManager.SetInteractableInMatrix(interactableSprites[random], random);
        }

        public void AssignTypeToInteractable(InteractableManager interactableManager, int id)
        {
            interactableManager.SetInteractableInMatrix(interactableSprites[id], id);
        }

        public void CycleInteractableType(InteractableManager interactableManager)
        {
            int currentTypeId = interactableManager.id;

            if (currentTypeId == howManyInteractables - 1)
            {
                AssignTypeToInteractable(interactableManager, 0);
            }
            else
            {
                AssignTypeToInteractable(interactableManager, currentTypeId + 1);
            }
        }

        public List<InteractableManager> GenerateMinimumTypeInteractables(int minPerType)
        {
            if (minPerType <= 0)
            {
                Debug.LogError("Minimum amount of each type shold be greater than zero");
                return null;
            }

            List<InteractableManager> interactables = new List<InteractableManager>(minPerType * howManyInteractables);
            for (int typeId = 0; typeId < howManyInteractables; typeId++)
            {
                for (int count = 0; count < minPerType; count++)
                {
                    InteractableManager newInteractableManager = GetObjectFromPool();
                    AssignTypeToInteractable(newInteractableManager, typeId);
                    interactables.Add(newInteractableManager);
                }
            }

            return interactables;
        }

        public List<InteractableManager> CreateInteractablesWithMinimumTypesAndRandomFill(int minEachType, int totalAmount)
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

            List<InteractableManager> interactables = new List<InteractableManager>(totalAmount);
            interactables.AddRange(GenerateMinimumTypeInteractables(minEachType));


            for (int i = 0; i < totalAmount - minEachType * howManyInteractables; i++)
            {
                InteractableManager newInteractableManager = GetObjectFromPool();
                AssignRandomTypeToInteractable(newInteractableManager);
                interactables.Add(newInteractableManager);
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

        List<InteractableManager> ArrangeInteractablesWithNoSameEachOther(List<InteractableManager> interactables)
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

        private void SwapInteractables(List<InteractableManager> interactables, int firstInteractable, int secondInteractable)
        {
            InteractableManager temp = interactables[firstInteractable];
            interactables[firstInteractable] = interactables[secondInteractable];
            interactables[secondInteractable] = temp;
        }

        public void ChangeInteractable(InteractableManager interactableManager)
        {
            int interactableCurrentId = interactableManager.id;
            if (interactableCurrentId != howManyInteractables - 1)
            {
                interactableCurrentId++;
            }
            else
            {
                interactableCurrentId = 0;
            }

            AssignTypeToInteractable(interactableManager, interactableCurrentId);
        }

        public void ClearInteractablePool()
        {
            Vector2Int defaultMatrixPosition = new Vector2Int(-1, -1);
            foreach (InteractableManager element in pool)
            {
                element.matrixPosition = defaultMatrixPosition;
                element.id = default;
            }

            ClearPool();
        }
    }
}