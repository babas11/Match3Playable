using Data.ValueObjects;
using DG.Tweening;
using Signals;
using UI;
using UnityEngine;


namespace Controllers.UI
{
    public class WinUIController : MonoBehaviour
    {
        
        #region Self Variables
        
        #region Private Variables
        
        private WinUIData _data;
        
        private SpriteRenderer starRenderer,
            victoryLabelRenderer,
            winBackGrounRenderer,
            continueButtonRenderer,
            transparentRenderer;

        private SpriteRenderer reTryLabelRenderer, reTryButtonRenderer, reTryTextRenderer, reTryBackGroundRenderer;
        
        #endregion
        
        #region Public Variables
        
        public CustomButton ContinueCustomButton { get; private set; }
        
        #endregion
        
        #endregion
        
        
        //Helper Screen Position Methods
        Vector3 GetBottomLeft() => Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
        Vector3 GetTopRight() =>
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));
        Vector3 GetCenter() {
            Vector3 bottomLeft = GetBottomLeft();
            Vector3 topRight = GetTopRight();
            return (bottomLeft + topRight) / 2f;
        }

    
        public void SetupWonUI(WinUIData data)
        {
            _data = data;
            Color defaultColor = Color.white;

            //Create All Sprite Elements and Set them to not enabled
            winBackGrounRenderer = SetUpSprite(_data.VictoryBackGroundSprite, _data.VictoryBackPosition, _data.VictoryBackScale, false, 4,
                defaultColor);
            starRenderer = SetUpSprite(_data.StarSprite, _data.WinStarPosition, _data.WinStarScale, false, 5, defaultColor);
            victoryLabelRenderer = SetUpSprite(_data.CompletedLabelSprite, _data.CompletedLabelPosition, _data.CompletedLabelScale, false,
                6, defaultColor);
            transparentRenderer = SetUpSprite(_data.TransparentBackGroundSprite, GetCenter(), _data.TransparentBackgroundScale, true, 3,
                new Color(0, 0, 0, 0));
            //Create Continue Button
            ContinueCustomButton = Instantiate(_data.ButtonPrefab).GetComponent<CustomButton>();
            ContinueCustomButton.SetUpButton(_data.ContinueButtonPosition, _data.ContinueLabelScale, _data.ContinueButtonSprite, 6, true, false);
            ContinueCustomButton.SetButtonAnimation(new Vector3(0.5f, 0.5f, 0.5f));
            ContinueCustomButton.onButtonDown.AddListener(GameSignals.Instance.onContinueButtonPressed);
        }


        public void RevealWonUI()
        {
            ResetPopUpElements();

            // Animate the color to full transparency
            Color fullTransparent = new Color(0, 0, 0, 0.95f); // Fully transparent color
            transparentRenderer.DOColor(fullTransparent, 0.5f);

            //Setting star's jump start position
            Vector3 jumpStartPosition = _data.WinStarPosition + new Vector3(-8, _data.WinStarPosition.y, 1);

            //Enabling and moving pop up background
            starRenderer.transform.position = jumpStartPosition;
            winBackGrounRenderer.gameObject.SetActive(true);
            winBackGrounRenderer.transform.DOMoveY(_data.VictoryBackPosition.y + 8, 0.8f).From().OnComplete(() =>
                {
                    //Enabling and moving win label after completion
                    victoryLabelRenderer.gameObject.SetActive(true);
                    victoryLabelRenderer.transform.DOMoveY(_data.CompletedLabelPosition.y + 8, 1.2f).From().OnComplete(() =>
                    {
                        //Enabling and moving win label after completion
                        starRenderer.gameObject.SetActive(true);
                        starRenderer.transform.DORotate(new Vector3(0, 0, 180), 0.5f, RotateMode.LocalAxisAdd)
                            .SetLoops(2).From().OnComplete(() =>
                            {
                                //Moving  star on Y and make a scale up-down
                                starRenderer.transform.DOMoveY(victoryLabelRenderer.transform.position.y, 0.4f)
                                    .SetEase(Ease.InQuad);
                                starRenderer.transform.DOScale(Vector3.one, 0.6f);

                                //Bringing continue button on scene
                                ContinueCustomButton.gameObject.SetActive(true);
                                ContinueCustomButton.transform.DOMoveY(_data.CompletedLabelPosition.y - 8, 0.8f).From();
                            });
                        //Making star a jump animation after labels animation
                        starRenderer.transform.DOJump(_data.WinStarPosition, 6f, 1, 1f);
                    });
                }
            );
        }

        private void ResetPopUpElements()
        {
            // Reset the transparentRenderer color to fully transparent
            transparentRenderer.color = new Color(1, 1, 1, 0);

            // Reset positions
            winBackGrounRenderer.transform.position = _data.VictoryBackPosition;
            victoryLabelRenderer.transform.position = _data.CompletedLabelPosition;
            starRenderer.transform.position = _data.WinStarPosition + new Vector3(-8, 0, 1); // Corrected the Y component
            ContinueCustomButton.transform.position = _data.ContinueButtonPosition;

            // Reset scales
            winBackGrounRenderer.transform.localScale = _data.VictoryBackScale;
            victoryLabelRenderer.transform.localScale = _data.CompletedLabelScale;
            starRenderer.transform.localScale = _data.WinStarScale;
            ContinueCustomButton.transform.localScale = _data.ContinueLabelScale;

            // Reset the game objects to inactive if they were active
            winBackGrounRenderer.gameObject.SetActive(false);
            victoryLabelRenderer.gameObject.SetActive(false);
            starRenderer.gameObject.SetActive(false);
            ContinueCustomButton.gameObject.SetActive(false);
        }

        public void RemoveWinUI()
        {
            Color fullTransparent = new Color(1, 1, 1, 0); // Fully transparent color
            transparentRenderer.DOColor(fullTransparent, 1f);


            winBackGrounRenderer.transform.DOMoveY(_data.VictoryBackPosition.y + 8, 0.4f).OnComplete(() =>
                {
                    //Enabling and moving win label after completion

                    victoryLabelRenderer.transform.DOMoveY(_data.CompletedLabelPosition.y + 8, 0.4f).OnComplete(() =>
                    {
                        starRenderer.transform.DOMoveY(_data.WinStarPosition.y + 8, .3f).SetEase(Ease.InQuad);

                        //Bringing continue button on scene
                        ContinueCustomButton.gameObject.SetActive(true);
                        ContinueCustomButton.transform.DOMoveY(_data.CompletedLabelPosition.y - 8, 0.8f).OnComplete(() =>
                        {
                            ContinueCustomButton.gameObject.SetActive(false);
                            starRenderer.gameObject.SetActive(false);
                            victoryLabelRenderer.gameObject.SetActive(false);
                            winBackGrounRenderer.gameObject.SetActive(false);
                        });
                    });
                }
            );
        }

        SpriteRenderer SetUpSprite(Sprite sprite, Vector3 position, Vector3 localScale, bool isActive, int sordingOrder,
            Color color)
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

    }
}