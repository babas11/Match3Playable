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
        uiManager.spinButton.onButtonDown.AddListener(() => grid.StartSpin());
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

                break;
            case GameStates.Spinning:
                break;
            case GameStates.MatchingActive:
                uiManager.RemoveSpinButton();
                //Selector Active
                break;
            case GameStates.Won:
            print("Won");
                //Selector Deactive
                //UI Pop Out
                break;
            case GameStates.Lost:
            print("You Lost");
                //Selector Deactive
                //UI Pop Out
                break;
        }
    }


}
public enum GameStates
{
    GameStart,
    SpinActive,
    Spinning,
    MatchingActive,
    Won,
    Lost
}