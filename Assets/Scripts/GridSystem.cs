using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridSystem<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    Vector2Int gridSize;
    public Vector2Int Dimensions => gridSize;

    T[,] gridArray;

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid(/* Vector2Int gridSize */)
    {
        if (gridSize.x <= 0 || gridSize.y <= 0)
        {
            Debug.LogError("Invalid grid size");
            return;
        }
        gridArray = new T[gridSize.x, gridSize.y];
    }

    public void PutItemOnGrid(T item, Vector2Int position)
    {
        if (IsInsideGrid(position))
        {
            gridArray[position.x, position.y] = item;
        }
        else
        {
            Debug.LogError("Position is outside of the grid");
        }
    }

    public T GetItemOnGrid(Vector2Int position)
    {
        if (IsInsideGrid(position))
        {
            return gridArray[position.x, position.y];
        }
        else
        {
            Debug.LogError("Position is outside of the grid");
            return null;
        }
    }

    public void RemoveItemFromGrid(Vector2Int position)
    {
        if (IsInsideGrid(position))
        {
            gridArray[position.x, position.y] = null;
        }
        else
        {
            Debug.LogError("Position is outside of the grid");
        }
    }

    private bool IsInsideGrid(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y)
        {
            return true;
        }
        else { return false; }
    }

    public Vector3 GridPositionToWorldPosition(Vector2Int position)
    {
        float xPosition = position.x * 0.6f + transform.position.x + .25f;

        float yPosition = position.y * 0.6f + transform.position.y + 0.25f;

        return new Vector3(xPosition, yPosition, 0);
    }

}
