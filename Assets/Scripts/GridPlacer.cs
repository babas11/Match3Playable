using UnityEngine;

[RequireComponent(typeof(InteractableGridSystem))]
public class GridPlacer : MonoBehaviour
{
    private void Awake()
    {
        interactableGridSystem = GetComponent<InteractableGridSystem>();
    }
    InteractableGridSystem interactableGridSystem;

    Vector2Int Dimensions => interactableGridSystem.Dimensions;


    float GridUnit => interactableGridSystem.GridSpacing;

    float GridWidth => Dimensions.x * GridUnit;
    float GridHeight => Dimensions.y * GridUnit;

    public GameObject backGround;
    public float backGroundGridScale = 1.1f;

    [SerializeField]
    Vector3 verticalOfscreenGridOffset = new Vector3(0, 0, 0);
    private float gridBottomOffset = 1f;

    public void PlaceGrid()
    {
        PositioningGridOnTheScreen();
        ResizeAndPlaceBackground();

    }

    private void ResizeAndPlaceBackground()
    {
        // Scale the background to be slightly larger than the grid
        float backgroundWidth = GridWidth * backGroundGridScale;
        float backgroundHeight = GridHeight * backGroundGridScale;

        // Set the background size
        SpriteRenderer bgRenderer = backGround.GetComponent<SpriteRenderer>();
        bgRenderer.size = new Vector2(backgroundWidth, backgroundHeight);

        // Calculate the background position
        Vector3 gridCenter = transform.position + new Vector3(GridWidth / 2f, GridHeight / 2f, 0f);

        //Remove half of the grid element size from position x and y to center the grid
        float halfGridUnit = GridUnit / 2f;
        gridCenter -= new Vector3(halfGridUnit, halfGridUnit, 0f);

        // Position the background at the grid center, adjusted for scaling
        backGround.transform.position = gridCenter  ;

    }

    private void PositioningGridOnTheScreen()
    {
        // Get screen bounds in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));

        float worldWidth = topRight.x - bottomLeft.x;

        //Add half of the grid element size
        float halfGridUnit = GridUnit / 2f;

        // Center the grid horizontally and apply vertical offset
        float gridXPosition = bottomLeft.x + (worldWidth - GridWidth)/ 2f + halfGridUnit;
        float gridYPosition = bottomLeft.y + gridBottomOffset + halfGridUnit;

        // Set the position of the grid
        transform.position = new Vector3(gridXPosition, gridYPosition, 0f)  + verticalOfscreenGridOffset;
    }
}