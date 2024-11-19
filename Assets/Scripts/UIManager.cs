using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    GameObject spinButtonPrefab;

    [SerializeField]
    public ButtonManager spinButton;

    [SerializeField]
    float buttonBottomDistance;

    [SerializeField]
    float buttonOffScreenDistance = 8f;

    [SerializeField]
    Sprite buttonImage;


    [SerializeField]
    Sprite[] moveCountSprites;
    SpriteRenderer moveCountSpriteRenderer;

    public int moveCount { get; private set; } = 5;



    public static UIManager Instance;


    Vector3 GetBottomLeft() => Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
    Vector3 GetTopRight() => Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));
    Vector3 GetTopLeft() => new Vector3(GetBottomLeft().x, GetTopRight().y, 0);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("An instance of" + typeof(UIManager) + "already exist in the scene. Self destructing");
            Destroy(gameObject);
        }
    }

    public void InitUI()
    {

        //create the Button
        CreateSpinButton();
        CreateMoveCount();
    }

    void CreateSpinButton()
    {
        GameObject buttonGO = Instantiate(spinButtonPrefab);
        spinButton = buttonGO.GetComponent<ButtonManager>();


        spinButton.SetUpButton(new Vector3(1.2f, 1.2f, 1), buttonImage);

        PositionSpinButtonOnCenterOfTheScreen(spinButton.transform, buttonBottomDistance - buttonOffScreenDistance);
    }

    void CreateMoveCount()
    {
        GameObject moveCountImage = new GameObject("MoveCount");
        moveCountSpriteRenderer = moveCountImage.AddComponent<SpriteRenderer>();
        moveCountSpriteRenderer.sortingOrder = 1;
        moveCountSpriteRenderer.sprite = moveCountSprites.Last();
        PositionMoveCountOnTheScreen(moveCountSpriteRenderer.transform, 0.5f);

    }

    public void MoveMade()
    {
        moveCount--;
        moveCountSpriteRenderer.sprite = moveCountSprites[moveCount];
    }

    private void PositionMoveCountOnTheScreen(Transform transformFrom, float distanceToTopLeftCorner)
    {
        Vector3 topLeftPosition = GetTopLeft();
        Vector3 newPosition = new Vector3(topLeftPosition.x + distanceToTopLeftCorner, topLeftPosition.y - distanceToTopLeftCorner, 0);
        transformFrom.position = newPosition;
    }

    private void PositionSpinButtonOnCenterOfTheScreen(Transform objectTransform, float distanceFromScreenBottom)
    {
        // Get the screen center in world units
        Vector3 bottomLeft = GetBottomLeft();
        Vector3 topRight = GetTopRight();

        // Calculate the center x position
        float buttonXPosition = (topRight.x + bottomLeft.x) / 2f; // Correctly centers the button

        // Set the vertical position relative to the bottom of the screen
        float buttonYPosition = bottomLeft.y + distanceFromScreenBottom;

        // Set the position of the button
        objectTransform.position = new Vector3(buttonXPosition, buttonYPosition, 0f);
    }


    public void RemoveSpinButton()
    {
        Vector3 startPos = spinButton.transform.position;
        Vector3 offScreenY = startPos - Vector3.down * 2;
        Vector3 startScale = spinButton.transform.localScale;
        spinButton.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        spinButton.transform.DOMoveY(-buttonOffScreenDistance, .8f));

    }
    public void RevealSpinButton()
    {
        Vector3 startPos = spinButton.transform.position;
        Vector3 offScreenY = startPos - Vector3.down * 2;
        spinButton.transform.DOMoveY(GetBottomLeft().y + buttonBottomDistance, 1f)
                            .OnComplete(() =>
        spinButton.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.1f).SetLoops(2, LoopType.Yoyo));



    }

}