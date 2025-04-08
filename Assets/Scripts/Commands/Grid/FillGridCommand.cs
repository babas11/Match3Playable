using System.Collections.Generic;
using UnityEngine;

namespace Commands.Grid
{
    public class FillGridCommand
    {
        private InteractableGridSystem _interactableGridSystem;
        
        public FillGridCommand(InteractableGridSystem interactableGridSystem)
        {
            _interactableGridSystem = interactableGridSystem;
        }

        internal void Execute(List<Interactable> interactables,Vector2Int dimensions,Transform gridTransform)
        {
            int index = 0;
            Vector3 startPosition = gridTransform.position;
            for (int x = 0; x < dimensions.x; x++)
            {
                startPosition.y = gridTransform.position.y;
                for (int y = 0; y < dimensions.y; y++)
                {
                    if (index >= interactables.Count)
                    {
                        Debug.LogError("Not enough interactables in the list to fill the grid.");
                        continue;
                    }
                    Interactable interactable = interactables[index];
                    if (interactable == null)
                    {
                        Debug.LogError("Interactable is null. Pool may not be properly initialized.");
                        continue;
                    }

                    interactable.matrixPosition = new Vector2Int(x, y);
                    _interactableGridSystem.PutItemOnGrid(interactable, new Vector2Int(x, y));
                    interactable.transform.position = new Vector3(startPosition.x, startPosition.y, 0);
                    interactable.gameObject.SetActive(true);
                    startPosition.y += _interactableGridSystem.GridSpacing;
                    index++;
                    /*  while (IsThereAnyMatch(interactable))
                     {
                         interactablePool.ChangeInteractable(interactable);
                     } */
                }

                startPosition.x += _interactableGridSystem.GridSpacing;
            }
        }
    }
}