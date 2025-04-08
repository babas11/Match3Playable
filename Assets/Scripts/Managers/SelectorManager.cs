using Grid;
using Signals;
using UnityEngine;

namespace Managers
{
    public class SelectorManager : MonoBehaviour
    {
        public static SelectorManager instance;
        InteractableManager[] selected;
        InteractableGridSystem grid;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning("An instance of" + typeof(SelectorManager) + "already exist in the scene. Self destructing");
                Destroy(gameObject);
            }

            selected = new InteractableManager[2];
        }

        private void Start()
        {
            grid = GameSignals.Instance.onGetGrid?.Invoke();
        }

        public void SelectFirst(InteractableManager toMatch)
        {
            selected[0] = toMatch;

            if(!enabled || selected[0] == null) return;

        }

        public void SelectSecond(InteractableManager toMatch)
        {
            selected[1] = toMatch;

            if(!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle) return;

            if(IsSelectedInteractablesAdjacent()){
                GameSignals.Instance.onSwap?.Invoke(selected);
                //print($"Swapping interactables at positions : ( {selected[0].matrixPosition.x},{selected[0].matrixPosition.y} and {selected[1].matrixPosition.x},{selected[1].matrixPosition.y})");
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
}