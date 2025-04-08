using Enums;
using Signals;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static GameStates State;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("An instance of" + typeof(SelectorManager) +
                                 "already exist in the scene. Self destructing");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            GameSignals.Instance.onGameInitialize?.Invoke();
            UpdateGameState(GameStates.GameStart);
        }


        public void UpdateGameState(GameStates newState)
        {
            State = newState;
            print(newState.ToString());
            switch (State)
            {
                case GameStates.GameStart:
                    GameSignals.Instance.onReadyToSpin?.Invoke();
                    break;
                case GameStates.SpinActive:
                    GameSignals.Instance.onSpinActive?.Invoke();
                    break;
                case GameStates.Spinning:
                    GameSignals.Instance.onsSpinning?.Invoke();
                    break;
                case GameStates.SpinEnd:
                    GameSignals.Instance.onSpinEnds?.Invoke();
                    break;
                case GameStates.MatchingActive:
                    GameSignals.Instance.onMatchingActive?.Invoke();
                    break;
                    //Selector Active
                case GameStates.LastMove:
                    //No grid input in this state
                    break;
                case GameStates.Won:
                    GameSignals.Instance.onWin?.Invoke();
                    break;
            }
        }

        void RestartGame()
        {
            GameSignals.Instance.onRestart.Invoke();
            UpdateGameState(GameStates.GameStart);
        }

        private void OnEnable()
        {
            GameSignals.Instance.onContinueButtonPressed += RestartGame;
        }


        private void OnDisable()
        {
            GameSignals.Instance.onContinueButtonPressed -= RestartGame;
        }
    }
}