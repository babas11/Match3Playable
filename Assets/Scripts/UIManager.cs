using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    GameObject buttonPrefab;

    public ButtonManager SpinButton { get; private set; }
    public ButtonManager ContinueButton { get; private set; }
    public ButtonManager ReTryButton { get; private set; }

    [SerializeField]
    float spinButtonBottomDistance;
    public int moveCount { get; private set; }

    public static UIManager Instance;


    //Helper Screen Position Methods
    Vector3 GetBottomLeft() => Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
    Vector3 GetTopRight() => Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));
    Vector3 GetTopLeft() => new Vector3(GetBottomLeft().x, GetTopRight().y, 0);
    Vector3 GetCenter() => GetTopRight() - GetBottomLeft();

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
        CreateSpinButton();
        CreateWonPopUp();
    }



    #region Spin Button
    [Header("Spin Button Settings")]
    [SerializeField]
    Vector3 spinButtonScale = new Vector3(0.3f, 0.3f, 1);
    [SerializeField]
    Vector3 spinButtonAnimationMaxScale = new Vector3(0.4f, 0.4f, 1);
    [SerializeField]
    Sprite buttonStartImage;
    [SerializeField]
    Sprite buttonStopImage;
    [SerializeField]
    float buttonOffScreenDistance = 8f;

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

    void CreateSpinButton()
    {
        GameObject buttonGO = Instantiate(buttonPrefab);
        SpinButton = buttonGO.GetComponent<ButtonManager>();
        SpinButton.SetUpButton(spinButtonScale, buttonStartImage, 1, true);
        SpinButton.SetButtonAnimation(spinButtonAnimationMaxScale);

        PositionSpinButtonOnCenterOfTheScreen(SpinButton.transform, spinButtonBottomDistance - buttonOffScreenDistance);
    }

    public void ActivateSpinButton(bool activate)
    {
        SpinButton.SetButtonActive(activate);

    }

    public void ChangeButtonImage(bool isSpinning)
    {
        Sprite sprite = isSpinning ? buttonStartImage : buttonStopImage;
        SpinButton.ChangeButtonImage(sprite);
    }


    public void RemoveSpinButton()
    {
        ActivateSpinButton(false);
        Vector3 startPos = SpinButton.transform.position;
        Vector3 offScreenY = startPos - Vector3.down * 2;
        Vector3 startScale = SpinButton.transform.localScale;
        SpinButton.transform.DOScale(spinButtonAnimationMaxScale, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        SpinButton.transform.DOMoveY(-buttonOffScreenDistance, .2f));

    }
    public void RevealSpinButton()
    {
        Vector3 startPos = SpinButton.transform.position;
        Vector3 offScreenY = startPos - Vector3.down * 2;
        ChangeButtonImage(true);
        ActivateSpinButton(true);
        SpinButton.transform.DOMoveY(GetBottomLeft().y + spinButtonBottomDistance, 1f)
                            .OnComplete(() =>
        SpinButton.transform.DOScale(spinButtonAnimationMaxScale, 0.4f).SetLoops(2, LoopType.Yoyo));
    }
    #endregion


    #region Pop Ups
    [Header("Pop Up Settings")]
    [SerializeField]
    Sprite victoryBackSprite;
    [SerializeField]
    Sprite winStarSprite;
    [SerializeField]
    Sprite completedLabelSprite;
    [SerializeField]
    Sprite continueSprite;
    [SerializeField]
    Sprite transparentBackGround;


    [SerializeField]
    Vector3 victoryBackPosition = new Vector3(0, 0.4f, 0), victoryBackScale = new Vector3(0.18f, 0.18f, 1f);

    [SerializeField]
    Vector3 completedLabelPosition = new Vector3(0, 1.5f, 0), completedLabelScale = new Vector3(0.23f, 0.23f, 1f);

    [SerializeField]
    Vector3 winStarPosition = new Vector3(0, 0, 0), winStarScale = new Vector3(0.5f, 0.5f, 1f);

    [SerializeField]
    Vector3 continuePosition = new Vector3(0, -1.9f, 0), continueLabelScale = new Vector3(0.42f, 0.42f, 1);




    [SerializeField]
    Vector3 transsparentScale = new Vector3(11, 11, 1);

    [SerializeField]
    Color transparentColor;





    SpriteRenderer starRenderer, victoryLabelRenderer, winBackGrounRenderer, continueButtonRenderer, transparentRenderer;

    SpriteRenderer reTryLabelRenderer, reTryButtonRenderer, reTryTextRenderer, reTryBackGroundRenderer;


    void CreateWonPopUp()
    {
        Color defaultColor = Color.white;

        //Create All Sprite Elements and Set them to not enabled
        winBackGrounRenderer = SetUpSprite(victoryBackSprite, victoryBackPosition, victoryBackScale, false, 4, defaultColor);
        starRenderer = SetUpSprite(winStarSprite, winStarPosition, winStarScale, false, 5, defaultColor);
        victoryLabelRenderer = SetUpSprite(completedLabelSprite, completedLabelPosition, completedLabelScale, false, 6, defaultColor);
        transparentRenderer = SetUpSprite(victoryBackSprite, GetCenter(), transsparentScale, true, 3, new Color(1, 1, 1, 0));
        //Create Continue Button
        ContinueButton = Instantiate(buttonPrefab).GetComponent<ButtonManager>();
        ContinueButton.SetUpButton(continuePosition, continueLabelScale, continueSprite, 6, true, false);
        ContinueButton.SetButtonAnimation(new Vector3(0.5f, 0.5f, 0.5f));
    }



    public void RevealWonUI()
    {
        ResetPopUpElements();
        
        // Animate the color to full transparency
        Color fullTransparent = new Color(1, 1, 1, 0); // Fully transparent color
        transparentRenderer.DOColor(transparentColor, 0.5f);

        //Setting star's jump start position
        Vector3 jumpStartPosition = winStarPosition + new Vector3(-8, winStarPosition.y, 1);

        //Enabling and moving pop up background
        starRenderer.transform.position = jumpStartPosition;
        winBackGrounRenderer.gameObject.SetActive(true);
        winBackGrounRenderer.transform.DOMoveY(victoryBackPosition.y + 8, 0.8f).From().
            OnComplete(() =>
            {
                //Enabling and moving win label after completion
                victoryLabelRenderer.gameObject.SetActive(true);
                victoryLabelRenderer.transform.DOMoveY(completedLabelPosition.y + 8, 1.2f).From().
            OnComplete(() =>
            {
                //Enabling and moving win label after completion
                starRenderer.gameObject.SetActive(true);
                starRenderer.transform.DORotate(new Vector3(0, 0, 180), 0.5f, RotateMode.LocalAxisAdd).SetLoops(2).From().
                OnComplete(() =>
                {
                    //Moving  star on Y and make a scale up-down
                    starRenderer.transform.DOMoveY(victoryLabelRenderer.transform.position.y, 0.4f).SetEase(Ease.InQuad);
                    starRenderer.transform.DOScale(Vector3.one, 0.6f);

                    //Bringing continue button on scene
                    ContinueButton.gameObject.SetActive(true);
                    ContinueButton.transform.DOMoveY(completedLabelPosition.y - 8, 0.8f).From();

                });
                //Making star a jump animation after labels animation
                starRenderer.transform.DOJump(winStarPosition, 6f, 1, 1f);
            });
            }
        );

    }

    private void ResetPopUpElements()
    {
        // Reset the transparentRenderer color to fully transparent
        transparentRenderer.color = new Color(1, 1, 1, 0);

        // Reset positions
        winBackGrounRenderer.transform.position = victoryBackPosition;
        victoryLabelRenderer.transform.position = completedLabelPosition;
        starRenderer.transform.position = winStarPosition + new Vector3(-8, 0, 1); // Corrected the Y component
        ContinueButton.transform.position = continuePosition;

        // Reset scales
        winBackGrounRenderer.transform.localScale = victoryBackScale;
        victoryLabelRenderer.transform.localScale = completedLabelScale;
        starRenderer.transform.localScale = winStarScale;
        ContinueButton.transform.localScale = continueLabelScale;

        // Reset the game objects to inactive if they were active
        winBackGrounRenderer.gameObject.SetActive(false);
        victoryLabelRenderer.gameObject.SetActive(false);
        starRenderer.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
    }

    public void RemoveWinUI()
    {
        Color fullTransparent = new Color(1, 1, 1, 0); // Fully transparent color
        transparentRenderer.DOColor(fullTransparent, 1f);


        winBackGrounRenderer.transform.DOMoveY(victoryBackPosition.y + 8, 0.4f).
            OnComplete(() =>
            {
                //Enabling and moving win label after completion

                victoryLabelRenderer.transform.DOMoveY(completedLabelPosition.y + 8, 0.4f).
            OnComplete(() =>
            {
                starRenderer.transform.DOMoveY(winStarPosition.y + 8, .3f).SetEase(Ease.InQuad);

                //Bringing continue button on scene
                ContinueButton.gameObject.SetActive(true);
                ContinueButton.transform.DOMoveY(completedLabelPosition.y - 8, 0.8f).OnComplete(() =>
                {
                    ContinueButton.gameObject.SetActive(false);
                    starRenderer.gameObject.SetActive(false);
                    victoryLabelRenderer.gameObject.SetActive(false);
                    winBackGrounRenderer.gameObject.SetActive(false);

                });

            });
            }
        );



    }



    SpriteRenderer SetUpSprite(Sprite sprite, Vector3 position, Vector3 localScale, bool isActive, int sordingOrder, Color color)
    {
        GameObject newSprite = new GameObject();
        SpriteRenderer renderer = newSprite.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sordingOrder;
        newSprite.transform.position = position;
        newSprite.transform.localScale = localScale;
        newSprite.gameObject.SetActive(isActive);
        return renderer;
    }


    #endregion

}