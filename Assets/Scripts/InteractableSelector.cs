using System;
using Signals;
using UnityEngine;

public class InteractableSelector : MonoBehaviour
{
    public static InteractableSelector instance;
    Interactable[] selected;
    InteractableGridSystem grid;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("An instance of" + typeof(InteractableSelector) + "already exist in the scene. Self destructing");
            Destroy(gameObject);
        }

        selected = new Interactable[2];
    }

    private void Start()
    {
        grid = GameSignals.Instance.onGetGrid?.Invoke();
    }

    public void SelectFirst(Interactable toMatch)
    {
        selected[0] = toMatch;

        if(!enabled || selected[0] == null) return;

    }

    public void SelectSecond(Interactable toMatch)
    {
        selected[1] = toMatch;

        if(!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle) return;

        if(IsSelectedInteractablesAdjacent()){
            //StartCoroutine(grid.Swap(selected));
            print($"Swapping interactables at positions : ( {selected[0].matrixPosition.x},{selected[0].matrixPosition.y} and {selected[1].matrixPosition.x},{selected[1].matrixPosition.y})");
        }
        SelectFirst(null);
    }

    bool IsSelectedInteractablesAdjacent(){
        int differenceOnAxis;
        if(selected[0].matrixPosition.x == selected[1].matrixPosition.x && IsDifferenceEqualsOne(out differenceOnAxis, selected[0].matrixPosition.y, selected[1].matrixPosition.y) )
        {
            return true;
        }
        else if(selected[0].matrixPosition.y == selected[1].matrixPosition.y && IsDifferenceEqualsOne(out differenceOnAxis, selected[0].matrixPosition.x, selected[1].matrixPosition.x))
        {
            return true;
        }
        return false;
    }

     private bool IsDifferenceEqualsOne(out int result, int input1, int input2)
    {
        result = input1 - input2;
        return Mathf.Abs(input1 - input2) == 1;
    }


}