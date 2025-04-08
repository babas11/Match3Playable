using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class GridSystem<T>  where T : MonoBehaviour
{
    Vector2Int gridSize;
    public Vector2Int Dimensions => gridSize;
    public int GridCapacity => gridSize.x * gridSize.y;

    T[,] gridArray;

    

    public void CreateGrid()
    {
        gridSize = SetGridSize();

        if (gridSize.x <= 0 || gridSize.y <= 0)
        {
            Debug.LogError("Invalid grid size");
            return;
        }
        gridArray = new T[gridSize.x, gridSize.y];
    }

    //Assigning grid Size randomly
    private Vector2Int SetGridSize()
    {
        int dimension = Random.Range(0,2) == 0 ? 7 : 5;
        return new Vector2Int(dimension,dimension);
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
    public T GetItemOnGrid(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);

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

    public bool IsInsideGrid(Vector2Int position)
    {
        if (position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y)
        {
            return true;
        }
        else { return false; }
    }

    public bool IsEmpty(Vector2Int position)
    {

        if (!IsInsideGrid(position))
        {
            Debug.LogError("Position is outside of the grid");
        }

        return EqualityComparer<T>.Default.Equals(gridArray[position.x, position.y], default(T));

    }

    public Vector3 GridPositionToWorldPosition(Vector2Int position,Transform gridTransform)
    {
        float xPosition = gridTransform.position.x + position.x * 0.6f;

        float yPosition = gridTransform.position.y + position.y * 0.6f;

        return new Vector3(xPosition, yPosition, 0);
    }

    public List<T> GetColumn(int columnIndex)
    {

        if (columnIndex >= Dimensions.x || columnIndex < 0)
        {
            Debug.LogError("Invalid column index");
            throw new ArgumentOutOfRangeException();
        }

        List<T> column = new List<T>();

        for (int i = 0; i < Dimensions.y; i++)
        {
            column.Add(GetItemOnGrid(columnIndex, i));

        }

        return column;

    }

    public void ChangeItemsAt(Vector2Int position1, Vector2Int position2)
    {
        if (!IsInsideGrid(position1)){Debug.LogError($"{position1} is outside of the grid");}
        if (!IsInsideGrid(position2)){Debug.LogError($"{position2} is outside of the grid");}
        
        T temp = gridArray[position1.x,position1.y];

        gridArray[position1.x,position1.y] = gridArray[position2.x,position2.y];
        gridArray[position2.x,position2.y] = temp;
    }

    public void ClearGrid(){
        gridArray = null;
    }
}
