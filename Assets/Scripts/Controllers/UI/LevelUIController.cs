using Data.ValueObjects;
using DG.Tweening;
using UI;
using UnityEngine;

namespace Controllers.UI
{
    public class LevelUIController : MonoBehaviour
    {
        
        #region Self Variables
        
        #region Private Variables
        
        private LevelUIData _data;
        
        #endregion
        
        #region Public Variables
        
        public CustomButton SpinButton { get; private set; }
        
        #endregion
        
        #endregion
        
        
        //Helper Screen Position Methods
        Vector3 GetBottomLeft() => Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
        Vector3 GetTopRight() => Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));
        
        
        public void SetupLevelUI(LevelUIData data)
        {
            _data = data;
            GameObject buttonGO = Instantiate(_data.ButtonPrefab);
            SpinButton = buttonGO.GetComponent<CustomButton>();
            SpinButton.SetUpButton(_data.SpinButtonScale, _data.ButtonStartImage, 1, true);
            SpinButton.SetButtonAnimation(_data.SpinButtonMaxAnimScale);
            PositionSpinButtonOnCenterOfTheScreen(SpinButton.transform, _data.SpinButtonDistance - _data.ButtonOffScreenDistance);
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
    
        public void ActivateSpinButton(bool activate)
        {
            SpinButton.SetButtonActive(activate);
        }
        
        public void ChangeButtonImage(bool isSpinning)
        {
            Sprite sprite = isSpinning ? _data.ButtonStartImage : _data.ButtonStopImage;
            SpinButton.ChangeButtonImage(sprite);
        }
        
        public void RemoveSpinButton()
        {
            ActivateSpinButton(false);
            SpinButton.transform.DOScale(_data.SpinButtonMaxAnimScale, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                SpinButton.transform.DOMoveY(-_data.ButtonOffScreenDistance, .2f));
        }
        
        public void RevealSpinButton()
        {
            ChangeButtonImage(true);
            ActivateSpinButton(true);
            SpinButton.transform.DOMoveY(GetBottomLeft().y + _data.SpinButtonDistance, 1f)
                .OnComplete(() =>
                    SpinButton.transform.DOScale(_data.SpinButtonMaxAnimScale, 0.4f).SetLoops(2, LoopType.Yoyo));
        }
    }
}