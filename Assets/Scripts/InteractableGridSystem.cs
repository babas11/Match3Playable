using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    public InteractablePool interactablePool;

    Coroutine[] spinCoroutines;

    [Tooltip("Distance between each grid cell")]
    [SerializeField]
    private float gridSpacing = 0.6f;
    public float GridSpacing => gridSpacing;


    [SerializeField]
    private float currentSpinSpeed = 10f;

    private float spinSpeedBottomLimit = 10f;
    private float deceleration = 1f;
    private int minimumAmountOfEachType = 3;

    private enum SpinState
    {
        Idle,
        Spinning,
        Decelerating
    }

    private SpinState[] columnSpinStates;
    private float[] currentSpinSpeeds;

    private SpinState spinState = SpinState.Idle;

    public void SetUpGrid()
    {
        CreateGrid();

        if (!interactablePool.Ready)
        {
            interactablePool.InitializePoolForGrid(Dimensions);
        }

        GetComponent<GridPlacer>().PlaceGrid();

        List<Interactable> interactablesToFill = interactablePool.CreateInteractablesWithMinimumTypesAndRandomFill(minimumAmountOfEachType, GridCapacity);

        FillGrid(interactablesToFill);
        spinCoroutines = new Coroutine[Dimensions.x];

        columnSpinStates = new SpinState[Dimensions.x];
        currentSpinSpeeds = new float[Dimensions.x];
        for (int i = 0; i < Dimensions.x; i++)
        {
            columnSpinStates[i] = SpinState.Idle;
            currentSpinSpeeds[i] = currentSpinSpeed;
        }
    }

    public void FillGrid(List<Interactable> interactables)
    {
        int index = 0;
        Vector3 startPosition = transform.position;
        for (int x = 0; x < Dimensions.x; x++)
        {
            startPosition.y = transform.position.y;
            for (int y = 0; y < Dimensions.y; y++)
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
                PutItemOnGrid(interactable, new Vector2Int(x, y));
                interactable.transform.position = new Vector3(startPosition.x, startPosition.y, 0);
                interactable.gameObject.SetActive(true);
                startPosition.y += gridSpacing;
                index++;
               /*  while (IsThereAnyMatch(interactable))
                {
                    interactablePool.ChangeInteractable(interactable);
                } */
            }

            startPosition.x += gridSpacing;
        }
    }

    public void ClearGrid()
    {
        StopAllCoroutines();
        Clear();
        interactablePool.ClearPool();
    }

    int CheckDirection(Vector2Int direction, Interactable interactable)
    {
        int matches = 0;
        Vector2Int position = direction + interactable.matrixPosition;

        while (IsInsideGrid(position) &&
               !IsEmpty(position) &&
                   GetItemOnGrid(position).id == interactable.id)
        {
            matches++;
            position += direction;

        }

        return matches;
    }



    private bool IsThereAnyMatch(Interactable interactableToCheck, bool checkForHorizontal = true, bool checkForVertical = true)
    {
        if (checkForHorizontal)
        {
            int matchinInterablesHorizontal = 0;
            //Look For Left
            Vector2Int leftDirection = Vector2Int.left;
            matchinInterablesHorizontal += CheckDirection(leftDirection, interactableToCheck);

            //Look for right
            Vector2Int rightDirection = Vector2Int.right;
            matchinInterablesHorizontal += CheckDirection(rightDirection, interactableToCheck);

            if (matchinInterablesHorizontal > 0) { return true; }

        }

        if (checkForVertical)
        {
            int matchingInteractablesVertical = 0;
            //Look For Up
            Vector2Int upDirection = Vector2Int.up;
            matchingInteractablesVertical += CheckDirection(upDirection, interactableToCheck);

            //Look for Down
            Vector2Int downDirection = Vector2Int.down;
            matchingInteractablesVertical += CheckDirection(downDirection, interactableToCheck);

            if (matchingInteractablesVertical > 0) { return true; }

        }

        return false;

    }




    #region Spin Mechanics

    public void StartSpin()
    {


        switch (GameManager.State)
        {
            case GameStates.SpinActive:
            case GameStates.GameStart:

                GameManager.Instance.UpdateGameState(GameStates.Spinning);
                StartCoroutine(SpinColumns());
                break;
            case GameStates.Spinning:
                StartCoroutine(DecelerateColumns());
                break;
        }
    }


    IEnumerator DecelerateColumns()
    {
        UIManager.Instance.ActivateSpinButton(false);
        for (int i = 0; i < Dimensions.x; i++)
        {
            columnSpinStates[i] = SpinState.Decelerating;

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator SpinColumns()
    {
        for (int i = 0; i < Dimensions.x; i++)
        {
            currentSpinSpeeds[i] = currentSpinSpeed;
            columnSpinStates[i] = SpinState.Idle;
        }
        for (int i = 0; i < Dimensions.x; i++)
        {
            columnSpinStates[i] = SpinState.Spinning;
            SpinColumn(i);
            yield return new WaitForSeconds(0.15f);
        }
    }



    void SpinColumn(int columnIndex)
    {

        List<Interactable> column = GetColumn(columnIndex);

        spinCoroutines[columnIndex] = StartCoroutine(SpinColumn(column, columnIndex));
    }

    IEnumerator SpinColumn(List<Interactable> interactablesOfColumn, int columnIndex)
    {
        AddExtraElementOnTop(interactablesOfColumn, columnIndex);

        List<Interactable> interactablesToRemove = new List<Interactable>();

        while (columnSpinStates[columnIndex] == SpinState.Spinning || columnSpinStates[columnIndex] == SpinState.Decelerating)
        {
            Dictionary<Interactable, Vector3> targetPositions = SetTargetPositions(interactablesOfColumn);

            yield return StartCoroutine(MoveInteractablesToTarget(interactablesOfColumn, targetPositions, columnIndex));

            SetPositions(interactablesOfColumn, columnIndex, interactablesToRemove);

            RemoveOutGridInteractable(interactablesOfColumn, interactablesToRemove);

            if (columnSpinStates[columnIndex] != SpinState.Idle)
            {
                AddExtraElementOnTop(interactablesOfColumn, columnIndex);
            }
        }
    }

    private void RemoveOutGridInteractable(List<Interactable> interactablesOfColumn, List<Interactable> interactablesToRemove)
    {
        foreach (var interactable in interactablesToRemove)
        {
            interactable.matrixPosition = new Vector2Int(-1, -1);
            interactablesOfColumn.Remove(interactable);
            interactablePool.ReturnObjectToPool(interactable);
        }
        interactablesToRemove.Clear();
    }

    private void SetPositions(List<Interactable> interactablesOfColumn, int columnIndex, List<Interactable> interactablesToRemove)
    {
        foreach (var interactable in interactablesOfColumn.ToList())
        {
            Vector2Int currentGridPosition = interactable.matrixPosition;

            if (currentGridPosition == new Vector2Int(columnIndex, 0))
            {
                interactablesToRemove.Add(interactable);
                RemoveItemFromGrid(currentGridPosition);
                continue;
            }

            if (currentGridPosition != new Vector2Int(-1, -1))
                RemoveItemFromGrid(currentGridPosition);

            // Update matrix position
            SetMatrixPosition(columnIndex, interactable, currentGridPosition);

            // Add interactable to new grid position
            if (IsInsideGrid(interactable.matrixPosition))
            {
                PutItemOnGrid(interactable, interactable.matrixPosition);
            }
        }

        // After updating positions, check for matches and adjust spin state
        if (currentSpinSpeeds[columnIndex] <= spinSpeedBottomLimit)
        {
            List<Interactable> interactablesToCheck = interactablesOfColumn
                .Where(x => !interactablesToRemove.Contains(x))
                .ToList();

            if (AnyMatchWithLeftColumn(interactablesToCheck))
            {
                // Match found with left column, continue spinning
                Debug.Log($"Column {columnIndex}: Match found with left column. Continuing spin.");
                columnSpinStates[columnIndex] = SpinState.Decelerating;
            }
            else
            {
                if (columnIndex == 0)
                {
                    Debug.Log($"Column {columnIndex}: Stopping spin.");
                    columnSpinStates[columnIndex] = SpinState.Idle;

                }
                else if (columnSpinStates[columnIndex - 1] == SpinState.Idle)
                {
                    Debug.Log($"Column {columnIndex}: No Match, Stopping spin.");
                    columnSpinStates[columnIndex] = SpinState.Idle;
                    if (columnIndex == Dimensions.x - 1)
                    {
                        print("UpdateSpin");
                        GameManager.Instance.UpdateGameState(GameStates.SpinActive);
                    }
                }
            }

            if (columnSpinStates[columnIndex] == SpinState.Idle)
            {
                SetWorldPositions(columnIndex, interactablesToCheck);
            }
        }
    }

    private void SetWorldPositions(int columnIndex, List<Interactable> interactables)
    {
        foreach (var interactable in interactables)
        {
            interactable.transform.position = GridPositionToWorldPosition(interactable.matrixPosition);

        }
    }

    private void SetMatrixPosition(int columnIndex, Interactable interactable, Vector2Int currentGridPosition)
    {
        if (currentGridPosition == new Vector2Int(-1, -1))
        {
            interactable.matrixPosition = new Vector2Int(columnIndex, Dimensions.y - 1);

        }
        else
        {
            interactable.matrixPosition = new Vector2Int(currentGridPosition.x, currentGridPosition.y - 1);

        }
    }

    private bool AnyMatchWithLeftColumn(List<Interactable> interactablesToCheck)
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

    IEnumerator MoveInteractablesToTarget(List<Interactable> interactablesOfColumn, Dictionary<Interactable, Vector3> targetPositions, int columnIndex)
    {
        bool allReachedTarget = false;
        while (!allReachedTarget)
        {
            allReachedTarget = true;
            foreach (var interactable in interactablesOfColumn)
            {
                Vector3 targetPosition = targetPositions[interactable];
                interactable.transform.position = Vector3.MoveTowards(interactable.transform.position, targetPosition, currentSpinSpeeds[columnIndex] * Time.deltaTime);

                if (Vector3.Distance(interactable.transform.position, targetPosition) >= 0.01f)
                {
                    allReachedTarget = false;
                }

                if (columnSpinStates[columnIndex] == SpinState.Decelerating)
                {
                    currentSpinSpeeds[columnIndex] -= deceleration * Time.deltaTime;
                    if (currentSpinSpeeds[columnIndex] <= spinSpeedBottomLimit)
                    {
                        currentSpinSpeeds[columnIndex] = spinSpeedBottomLimit;
                    }
                }
            }

            yield return null;
        }
    }

    private Dictionary<Interactable, Vector3> SetTargetPositions(List<Interactable> interactablesOfColumn)
    {
        Dictionary<Interactable, Vector3> targetPositions = new Dictionary<Interactable, Vector3>();

        foreach (var interactable in interactablesOfColumn)
        {
            if (interactable == null)
            {
                Debug.LogWarning("Interactable is null in SetTargetPositions.");
                continue;
            }
            Vector3 targetPosition = interactable.transform.position - new Vector3(0, gridSpacing, 0);
            targetPositions[interactable] = targetPosition;
        }

        return targetPositions;
    }

    private void AddExtraElementOnTop(List<Interactable> interactablesOfColumn, int columnIndex)
    {
        var extraElement = interactablePool.GetObjectFromPool();

        if (extraElement == null)
        {
            Debug.LogError("Extra element is null. Pool may not be properly initialized.");
            return; // Handle this case appropriately
        }

        if (interactablesOfColumn.Count > 0 && interactablesOfColumn.First() != null && interactablesOfColumn.Last() != null)
        {
            // If the bottom and top element of the column are the same, they come together.
            // In order to avoid it, set the new element to the next id if there is a match.
            if (interactablesOfColumn.First().id == interactablesOfColumn.Last().id)
            {
                interactablePool.CycleInteractableType(extraElement);
            }
            else
            {
                interactablePool.AssignTypeToInteractable(extraElement, interactablesOfColumn.First().id);
            }
        }
        else
        {
            // Handle the case when 'interactablesOfColumn' is empty or has null entries
            interactablePool.AssignRandomTypeToInteractable(extraElement);
        }

        extraElement.gameObject.SetActive(true);
        extraElement.transform.position = GridPositionToWorldPosition(new Vector2Int(columnIndex, Dimensions.y - 1)) + new Vector3(0, gridSpacing, 0);

        interactablesOfColumn.Add(extraElement);
    }


    #endregion

    #region Swap Mechanics

    public IEnumerator Swap(Interactable[] interactables)
    {
        Interactable[] copies = new Interactable[2];
        copies[0] = interactables[0];
        copies[1] = interactables[1];

        ChangeItemsAt(copies[0].matrixPosition, copies[1].matrixPosition);

        Vector2Int temp = copies[0].matrixPosition;
        copies[0].matrixPosition = copies[1].matrixPosition;
        copies[1].matrixPosition = temp;

        List<Interactable> interactable0Matches;
        List<Interactable> interactable1Matches;

        interactable0Matches = GetMatches(copies[0]);
        interactable1Matches = GetMatches(copies[1]);

        if (interactable0Matches != null || interactable1Matches != null)
        {
            GameManager.Instance.UpdateGameState(GameStates.LastMove);
        }


        yield return StartCoroutine(AnimateSwap(copies));


        if (interactable0Matches != null || interactable1Matches != null)
        {
            GameManager.Instance.UpdateGameState(GameStates.Won);
        }

    }

    private List<Interactable> GetMatchesInDirection(Interactable toMatch, Vector2Int direction)
    {
        List<Interactable> interactables = new List<Interactable>();
        Vector2Int position = toMatch.matrixPosition + direction;
        Interactable next;

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



    private List<Interactable> GetMatches(Interactable interactable)
    {
        if (interactable == null || !IsInsideGrid(interactable.matrixPosition) || IsEmpty(interactable.matrixPosition)) return null;


        List<Interactable> matchesList = new List<Interactable>();
        matchesList.Add(interactable);

        List<Interactable> horizontalMatch, verticalMatch;

        horizontalMatch = GetMatchesInDirection(interactable, Vector2Int.left);
        horizontalMatch.AddRange(GetMatchesInDirection(interactable, Vector2Int.right));
        if (horizontalMatch.Count > 1)
        {
            matchesList.AddRange(horizontalMatch);
        }

        verticalMatch = GetMatchesInDirection(interactable, Vector2Int.up);
        verticalMatch.AddRange(GetMatchesInDirection(interactable, Vector2Int.down));
        if (verticalMatch.Count > 1)
        {
            matchesList.AddRange(verticalMatch);
        }

        if (matchesList.Count < 3) return null;

        return matchesList;



    }

    IEnumerator AnimateSwap(Interactable[] interactables)
    {
        Interactable interactable0 = interactables[0];
        Interactable interactable1 = interactables[1];

        print("animating");
        Vector3[] targetPositions = new Vector3[2];
        targetPositions[0] = interactable1.transform.position;
        targetPositions[1] = interactable0.transform.position;

        Vector3 scale = new Vector3(1.2f, 1.2f, 1f);

        // create tweens for scaling up
        Tween scaleUpTween0 = interactable0.transform.DOScale(scale, 0.5f)
            .SetEase(Ease.InQuad)
            .OnStart(() => interactable0.Idle = false);

        Tween scaleUpTween1 = interactable1.transform.DOScale(scale, 0.5f)
            .SetEase(Ease.InQuad)
            .OnStart(() => interactable1.Idle = false);

        // create tweens for moving
        Tween moveTween0 = interactable0.transform.DOMove(targetPositions[0], 1f)
            .SetEase(Ease.InQuad);

        Tween moveTween1 = interactable1.transform.DOMove(targetPositions[1], 1f)
            .SetEase(Ease.InQuad);

        // Create Tweens for scaling down
        Tween scaleDownTween0 = interactable0.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => interactable0.Idle = true);

        Tween scaleDownTween1 = interactable1.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => interactable1.Idle = true);

        // Create main sequence
        Sequence mainSequence = DOTween.Sequence();

        // Add scaling up tweens (sequence0)
        mainSequence.Append(scaleUpTween0);
        mainSequence.Join(scaleUpTween1);

        // Add moving tweens (sequence1)
        mainSequence.Append(moveTween0);
        mainSequence.Join(moveTween1);

        // Add scaling down tweens
        mainSequence.Append(scaleDownTween0);
        mainSequence.Join(scaleDownTween1);

        // Wait for the sequence to complete
        yield return mainSequence.WaitForCompletion();
    }





    #endregion


}





