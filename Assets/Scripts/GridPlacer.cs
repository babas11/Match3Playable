using UnityEngine;

[RequireComponent(typeof(InteractableGridSystem))]
public class GridPlacer : MonoBehaviour
{
    private void Awake() {
        interactableGridSystem = GetComponent<InteractableGridSystem>();
    }
    InteractableGridSystem interactableGridSystem;

    Vector2Int Dimensions => interactableGridSystem.Dimensions;
    float gridUnit => interactableGridSystem.GridSpacing;

    public GameObject backGround;
    public float backGroundGridScale = 1.1f;

    [SerializeField]
    Vector3 verticalOfscreenGridOffset = new Vector3(0, 0, 0);
    private float gridBottomOffset = 1.5f;

    void Start()
    {
        ResizeAndPlaceBackground();
    }

    private void ResizeAndPlaceBackground()
    {
        //Getting corners in order to calculate sizes of the scene in both direction in world unit
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        //calculating sizes of the scene in both direction in world unit
        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;

        // Calculate the width and height of the grid based on dimensions using grid sizes and unit sizes
        float gridWidth = interactableGridSystem.Dimensions.x * gridUnit;
        float gridHeight = Dimensions.y * gridUnit;

        // Increase the size slightly to ensure the background is larger than the grid
        float backgroundWidth = gridWidth * backGroundGridScale;
        float backgroundHeight = gridHeight * backGroundGridScale;

        // Calculate the difference between the background and grid sizes to align the background on grid perfectly
        float differenceInWidth = backgroundWidth - gridWidth;
        float differenceInHeight = backgroundHeight - gridHeight;

        // Set the background size to cover the entire grid
        backGround.GetComponent<SpriteRenderer>().size = new Vector2(backgroundWidth, backgroundHeight);
        //backGround.transform.localScale = new Vector3(backgroundWidth, backgroundHeight, 1f);

        // Get the bounds of the SpriteRenderer
        Bounds bounds = backGround.GetComponent<SpriteRenderer>().bounds;

        // Calculate the offset to align the bottom left corner of the background with the grid
        Vector3 offset = new Vector3(bounds.extents.x, bounds.extents.y, 0);

        // Set the position to match the target position, considering the offset
        backGround.transform.position = transform.position + offset + verticalOfscreenGridOffset - new Vector3(differenceInWidth, differenceInHeight) / 2f;

    }

    private void PositioningGridOnTheScreen()
    {
        // Get screen bounds in world units
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;
        float gridXPosition = (worldWidth - (Dimensions.x * .5f)) / 2f;
        float gridYPosition = gridBottomOffset;

        // Set the position of the grid to the bottom left
        transform.position = bottomLeft + new Vector3(gridXPosition, gridYPosition, 1f);
    }

}