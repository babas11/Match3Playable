using System;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public static GameStates State;

    InteractableGridSystem grid;
    InteractableSelector selector;
    UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("An instance of" + typeof(InteractableSelector) + "already exist in the scene. Self destructing");
            Destroy(gameObject);
        }
        grid = GameObject.FindObjectOfType<InteractableGridSystem>();
        selector = GameObject.FindObjectOfType<InteractableSelector>();
        uiManager = UIManager.Instance;



    }

    private void Start()
    {

        grid.SetUpGrid();
        uiManager.InitUI();
        uiManager.SpinButton.onButtonDown.AddListener(() => grid.StartSpin());
        uiManager.ContinueButton.onButtonDown.AddListener(() => RestartGame());

        UpdateGameState(GameStates.GameStart);
    }


    public void UpdateGameState(GameStates newState)
    {
        State = newState;
        print(newState.ToString());
        switch (State)
        {
            case GameStates.GameStart:
                uiManager.RevealSpinButton();
                break;
            case GameStates.SpinActive:
                uiManager.ChangeButtonImage(true);
                uiManager.ActivateSpinButton(true);
                break;
            case GameStates.Spinning:
                uiManager.ChangeButtonImage(false);
                break;
            case GameStates.MatchingActive:
                uiManager.RemoveSpinButton();
                //Selector Active
                break;
            case GameStates.LastMove:
                //No grid input in this state
                break;
            case GameStates.Won:
                print("Won");
                uiManager.RevealWonUI();
                break;
        }
    }

    void RestartGame(){
        uiManager.RemoveWinUI();
        grid.ClearGrid();
        grid.SetUpGrid();
        UpdateGameState(GameStates.GameStart);
    }


}
public enum GameStates
{
    GameStart,
    SpinActive,
    Spinning,
    MatchingActive,
    Won,
    LastMove
}