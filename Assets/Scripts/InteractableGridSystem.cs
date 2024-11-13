using UnityEngine;
using UnityEngine.Pool;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    InteractablePool interactablePool;
    
    [Tooltip("Distance between each grid cell")]
    [SerializeField]
    private float gridSpacing = 0.6f;
    public float GridSpacing => gridSpacing;

    private void Start()
    {
        CreateGrid();
        interactablePool.InitializePool(Dimensions);
        GetComponent<GridPlacer>().PlaceGrid();
        
        FillGrid();
    }

    public void FillGrid()
    {
        Vector3 startPosition = transform.position;
        for (int x = 0; x < Dimensions.x; x++)
        {
            for (int y = 0; y < Dimensions.y; y++)
            {
                Interactable interactable = interactablePool.GetObjectFromPool();
                interactablePool.SetInteractable(interactable);
                interactable.matrixPosition = new Vector2Int(x, y);
                PutItemOnGrid(interactable, new Vector2Int(x, y));
                interactable.transform.position = new Vector3(startPosition.x, startPosition.y , 0);
                interactable.gameObject.SetActive(true);
                startPosition.y += gridSpacing;
            }
            startPosition.x += gridSpacing;
            startPosition.y = transform.position.y;
        }
    }

    
    
}