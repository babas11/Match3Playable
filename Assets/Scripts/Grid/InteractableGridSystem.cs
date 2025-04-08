using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Grid
{
    public class InteractableGridSystem : GridSystem<InteractableManager>
    {
        [Tooltip("Distance between each grid cell")]
        [SerializeField]
        private float gridSpacing = 0.6f;
        public float GridSpacing => gridSpacing;
    
    
        public void SetWorldPositions(int columnIndex, List<InteractableManager> interactables,Transform transform)
        {
            foreach (var interactable in interactables)
            {
                interactable.transform.position = GridPositionToWorldPosition(interactable.matrixPosition,transform);

            }
        }

        public void SetMatrixPosition(int columnIndex, InteractableManager interactableManagerManager, Vector2Int currentGridPosition)
        {
            if (currentGridPosition == new Vector2Int(-1, -1))
            {
                interactableManagerManager.matrixPosition = new Vector2Int(columnIndex, Dimensions.y - 1);

            }
            else
            {
                interactableManagerManager.matrixPosition = new Vector2Int(currentGridPosition.x, currentGridPosition.y - 1);

            }
        }
        public bool AnyMatchWithLeftColumn(List<InteractableManager> interactablesToCheck)
        {
            foreach (var interactableToCheck in interactablesToCheck)
            {
                Vector2Int onLeftPosition = interactableToCheck.matrixPosition + Vector2Int.left;

                if (!IsInsideGrid(onLeftPosition))
                {
                    return false;
                }

                if (GetItemOnGrid(onLeftPosition).id == interactableToCheck.id)
                {
                    return true;
                }
            }
            return false;
        }
    
        private List<InteractableManager> GetMatchesInDirection(InteractableManager toMatch, Vector2Int direction)
        {
            List<InteractableManager> interactables = new List<InteractableManager>();
            Vector2Int position = toMatch.matrixPosition + direction;
            InteractableManager next;

            while (IsInsideGrid(position) && !IsEmpty(position))
            {
                next = GetItemOnGrid(position);

                if (next.id == toMatch.id && next.Idle)
                {
                    interactables.Add(GetItemOnGrid(position));
                    position += direction;
                }
                else break;

            }

            return interactables;
        }

    
        public List<InteractableManager> GetMatches(InteractableManager interactableManagerManager)
        {
            if (interactableManagerManager == null || !IsInsideGrid(interactableManagerManager.matrixPosition) || IsEmpty(interactableManagerManager.matrixPosition)) return null;


            List<InteractableManager> matchesList = new List<InteractableManager>();
            matchesList.Add(interactableManagerManager);

            List<InteractableManager> horizontalMatch, verticalMatch;

            horizontalMatch = GetMatchesInDirection(interactableManagerManager, Vector2Int.left);
            horizontalMatch.AddRange(GetMatchesInDirection(interactableManagerManager, Vector2Int.right));
            if (horizontalMatch.Count > 1)
            {
                matchesList.AddRange(horizontalMatch);
            }

            verticalMatch = GetMatchesInDirection(interactableManagerManager, Vector2Int.up);
            verticalMatch.AddRange(GetMatchesInDirection(interactableManagerManager, Vector2Int.down));
            if (verticalMatch.Count > 1)
            {
                matchesList.AddRange(verticalMatch);
            }

            if (matchesList.Count < 3) return null;

            return matchesList;
        }
    }
}





