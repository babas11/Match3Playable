using Controllers.UI;
using Data.UnityObjects;
using Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Self Variables
        
        #region Private Variables
        
        private UnityAction spinButtonAction;
        private UnityAction continueButtonAction;
        private CD_UI _data;
        
        #endregion
        
        #region Serialized Variables
        
        [SerializeField] private LevelUIController levelUIController;
        [SerializeField] private WinUIController winUIController;
        
        #endregion
        
        #region Singleton
        
        public static UIManager Instance;
        
        #endregion
        
        #endregion
        
        private void Awake()
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

            GetData();
            InitUI();
        }
        
        private void OnEnable()
        {
            GameSignals.Instance.onGameInitialize += InitUI;
            GameSignals.Instance.onReadyToSpin += OnRedyToSpin;
            GameSignals.Instance.onRestart += OnRestart;
            GameSignals.Instance.onSpinActive += OnSpinActive;
            GameSignals.Instance.onsSpinning += OnSpinning;
            GameSignals.Instance.onSpinEnds += OnSpinEnd;
            GameSignals.Instance.onMatchingActive += OnMatchingActive;
            GameSignals.Instance.onWin += OnWin;

        }
        
        private void InitUI()
        {
            levelUIController.SetupLevelUI(_data.LevelUIData);
            winUIController.SetupWonUI(_data.WinUIData);
            spinButtonAction = GameSignals.Instance.onSpinButtonPressed;
            continueButtonAction = GameSignals.Instance.onContinueButtonPressed;
            levelUIController.SpinButton.onButtonDown.AddListener(spinButtonAction);
            winUIController.ContinueCustomButton.onButtonDown.AddListener(continueButtonAction);
        }
        
        private void OnRedyToSpin()
        {
            levelUIController.RevealSpinButton();
        }
        
        private void OnRestart()
        {
            winUIController.RemoveWinUI();
        }
        private void OnSpinActive()
        {
            levelUIController.ChangeButtonImage(true);
            levelUIController.ActivateSpinButton(true);
        }

        private void OnSpinning()
        {
            //levelUIController.ActivateSpinButton(false);
            levelUIController.ChangeButtonImage(false);
        }
        private void OnSpinEnd()
        {
            levelUIController.ActivateSpinButton(true);
        }

        private void OnMatchingActive()
        {
            levelUIController.RemoveSpinButton();
        }

        private void OnWin()
        {
            winUIController.RevealWonUI();
        }
        private void OnDisable()
        {
            GameSignals.Instance.onGameInitialize -= InitUI;
            GameSignals.Instance.onReadyToSpin -= OnRedyToSpin;
            GameSignals.Instance.onRestart -= OnRestart;
            GameSignals.Instance.onSpinActive -= OnSpinActive;
            GameSignals.Instance.onsSpinning -= OnSpinning;
            GameSignals.Instance.onSpinEnds -= OnSpinEnd;
            GameSignals.Instance.onMatchingActive -= OnMatchingActive;
            GameSignals.Instance.onWin -= OnWin;
            levelUIController.SpinButton.onButtonDown.RemoveListener(spinButtonAction);
            winUIController.ContinueCustomButton.onButtonDown.RemoveListener(continueButtonAction);
        }
        
        private void GetData()
        {
            _data = Resources.Load<CD_UI>("Data/UI/CD_UI");
        }
        
    }
    
    
}