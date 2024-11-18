using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private bool isSpinning = false;

    [SerializeField]
    private float currentSpinSpeed = 5f; // Adjust as needed

    private float spinSpeedBottomLimit = 1f;
    private float deceleration = 1f;

    private enum SpinState
    {
        Idle,
        Spinning,
        Decelerating
    }

    private SpinState spinState = SpinState.Idle;
    private int activeCoroutines = 0;

    private void Start()
    {
        CreateGrid();
        interactablePool.InitializePool(Dimensions);
        GetComponent<GridPlacer>().PlaceGrid();
        FillGrid();
        spinCoroutines = new Coroutine[Dimensions.x];
    }

    public void FillGrid()
    {
        Vector3 startPosition = transform.position;
        for (int x = 0; x < Dimensions.x; x++)
        {
            startPosition.y = transform.position.y; // Reset Y position for each column
            for (int y = 0; y < Dimensions.y; y++)
            {
                Interactable interactable = interactablePool.GetObjectFromPool();
                interactablePool.SetInteractable(interactable);
                interactable.matrixPosition = new Vector2Int(x, y);
                PutItemOnGrid(interactable, new Vector2Int(x, y));
                interactable.transform.position = new Vector3(startPosition.x, startPosition.y, 0);
                interactable.gameObject.SetActive(true);
                startPosition.y += gridSpacing;
                while (IsThereAnyMatch(interactable))
                {
                    interactablePool.ChangeInteractable(interactable);
                }
            }
            startPosition.x += gridSpacing;
        }
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

    public void StartSpin()
    {

        switch (spinState)
        {
            case SpinState.Idle:

                spinState = SpinState.Spinning;
                StartCoroutine(SpinColumns());

                break;
            case SpinState.Spinning:
                spinState = SpinState.Decelerating;
                break;
        }


    }

    IEnumerator SpinColumns()
    {
        for (int i = 0; i < Dimensions.x; i++)
        {

            List<Interactable> column = GetColumn(i);
            SpinColumn(i);
            yield return new WaitForSeconds(.3f);
        }

    }



    public void SpinColumn(int columnIndex)
    {
        List<Interactable> column = GetColumn(columnIndex);

        spinCoroutines[columnIndex] = StartCoroutine(SpinColumn(column, columnIndex));
    }

    IEnumerator SpinColumn(List<Interactable> interactablesOfColumn, int columnIndex)
    {
        AddExtraElementOnTop(interactablesOfColumn, columnIndex);

        List<Interactable> interactablesToRemove = new List<Interactable>();


        while (spinState == SpinState.Spinning || spinState == SpinState.Decelerating)
        {

            Dictionary<Interactable, Vector3> targetPositions = SetTargetPositions(interactablesOfColumn);

            yield return StartCoroutine(MoveInteractablesToTarget(interactablesOfColumn, targetPositions));

            if (spinState == SpinState.Idle)
            {
                print("Exited while loop");
            }

            SetPositions(interactablesOfColumn, columnIndex, interactablesToRemove);
            
            RemoveOutGridInteractable(interactablesOfColumn, interactablesToRemove);

            if (spinState != SpinState.Idle)
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
            }
            else if (currentGridPosition == new Vector2Int(-1, -1))
            {
                interactable.matrixPosition = new Vector2Int(columnIndex, Dimensions.y - 1);
                if (spinState == SpinState.Idle)
                {
                    print("Exited while loop");
                    interactable.transform.position = GridPositionToWorldPosition(new Vector2Int(columnIndex, Dimensions.y - 1));
                }

            }
            else
            {

                interactable.matrixPosition = new Vector2Int(currentGridPosition.x, currentGridPosition.y - 1);
                if (spinState == SpinState.Idle)
                {
                    interactable.transform.position = GridPositionToWorldPosition(interactable.matrixPosition);
                }

            }
        }
    }

    IEnumerator MoveInteractablesToTarget(List<Interactable> interactablesOfColumn, Dictionary<Interactable, Vector3> targetPositions)
    {
        bool allReachedTarget = false;
        while (!allReachedTarget)
        {
            allReachedTarget = true;
            foreach (var interactable in interactablesOfColumn)
            {
                Vector3 targetPosition = targetPositions[interactable];
                interactable.transform.position = Vector3.MoveTowards(interactable.transform.position, targetPosition, currentSpinSpeed * Time.deltaTime);

                if (Vector3.Distance(interactable.transform.position, targetPosition) >= 0.01f)
                {
                    allReachedTarget = false;
                }

                if (spinState == SpinState.Decelerating)
                {
                    currentSpinSpeed -= deceleration * Time.deltaTime;
                    if (spinState == SpinState.Decelerating && currentSpinSpeed <= spinSpeedBottomLimit)
                    {
                        spinState = SpinState.Idle;
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
            Vector3 targetPosition = interactable.transform.position - new Vector3(0, gridSpacing, 0);
            targetPositions[interactable] = targetPosition;
        }

        return targetPositions;
    }

    private void AddExtraElementOnTop(List<Interactable> interactablesOfColumn, int columnIndex)
    {
        var extraElement = interactablePool.GetObjectFromPool();
        interactablePool.SetInteractable(extraElement, interactablesOfColumn.First().id);
        extraElement.gameObject.SetActive(true);
        extraElement.transform.position = GridPositionToWorldPosition(new Vector2Int(columnIndex, Dimensions.y - 1)) + new Vector3(0, gridSpacing, 0);
        interactablesOfColumn.Add(extraElement);
    }
}





/*

public void StartSpin()
{
    if (spinState != SpinState.Idle)
    {
        print("Cannot start spin: Spin is already in progress or decelerating");
        return;
    }

    isSpinning = true;
    spinState = SpinState.Spinning;
    print("entered start spin");

    for (int x = 0; x < Dimensions.x; x++)
    {
        SpinColumn(x);
    }
}

public void SpinColumn(int columnIndex)
{
    print($"entered start column {columnIndex}");

    List<Interactable> columnItems = new List<Interactable>();

    // Collect all items in the column
    for (int y = 0; y < Dimensions.y; y++)
    {
        Interactable item = GetItemOnGrid(new Vector2Int(columnIndex, y));
        columnItems.Add(item);
    }

    activeCoroutines++; // Increment the counter
    StartCoroutine(SpinColumnCoroutine(columnItems, columnIndex));
}

private IEnumerator SpinColumnCoroutine(List<Interactable> columnItems, int columnIndex)
{
    print("entered coroutine");
    float currentSpeed = spinSpeed;

    // Calculate the height of one cell
    float cellHeight = gridSpacing;

    // While the spin is active
    while (isSpinning)
    {
        // Move each item in the column downward
        foreach (var item in columnItems)
        {
            item.transform.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

            // If the item has moved out of the grid at the bottom
            if (item.transform.position.y < transform.position.y - cellHeight)
            {
                // Move it to the top
                item.transform.position += new Vector3(0, Dimensions.y * cellHeight, 0);
            }
        }

        yield return null;
    }

    // Decelerate the spin
    while (currentSpeed > 0)
    {
        currentSpeed -= deceleration * Time.deltaTime;
        currentSpeed = Mathf.Max(currentSpeed, 0);

        foreach (var item in columnItems)
        {
            item.transform.position -= new Vector3(0, currentSpeed * Time.deltaTime, 0);

            if (item.transform.position.y < transform.position.y - cellHeight)
            {
                item.transform.position += new Vector3(0, Dimensions.y * cellHeight, 0);
            }
        }

        yield return null;
    }

    // Align items to grid positions after stopping
    AlignColumnToGrid(columnItems, columnIndex);

    // Decrement the counter
    activeCoroutines--;

    // If all coroutines have finished, set spinState to Idle
    if (activeCoroutines == 0)
    {
        spinState = SpinState.Idle;
        print("All columns have finished decelerating and aligning");
    }
}

private void AlignColumnToGrid(List<Interactable> columnItems, int columnIndex)
{
    // Sort items by their Y position (from top to bottom)
    columnItems.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

    for (int y = 0; y < Dimensions.y; y++)
    {
        Interactable item = columnItems[y];
        Vector2Int gridPosition = new Vector2Int(columnIndex, y);
        Vector3 targetPosition = GridPositionToWorldPosition(gridPosition);
        item.transform.position = targetPosition;

        // Update the item's matrix position
        item.matrixPosition = gridPosition;

        // Update the grid array
        PutItemOnGrid(item, gridPosition);
    }
}

public void StopSpin()
{
    if (spinState != SpinState.Spinning)
    {
        print("Cannot stop spin: Spin is not in progress");
        return;
    }

    isSpinning = false;
    spinState = SpinState.Decelerating;
    print("Stopping spin");
}

public void Spin()
{
    if (spinState == SpinState.Idle)
    {
        print("Spin button pressed: starting spin");
        StartSpin();
    }
    else if (spinState == SpinState.Spinning)
    {
        print("Spin button pressed: stopping spin");
        StopSpin();
    }
    else
    {
        print("Spin is decelerating. Please wait until it stops completely.");
    }
}
*/
