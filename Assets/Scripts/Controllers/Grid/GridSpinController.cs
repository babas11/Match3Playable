using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.UnityObjects;
using Enums;
using Grid;
using Managers;
using Signals;
using UnityEngine;

namespace Controllers.Grid
{
    public class GridSpinController : MonoBehaviour
    {

        #region Self Variables

        #region Private Variables

        private CD_Grid _data;
        private Coroutine[] _spinCoroutines;
        private InteractableGridSystem _interactableGridSystem;
        private Vector2Int Dimensions => _interactableGridSystem.Dimensions;
        private float GridSpacing => _data.GridViewData.GridUnit;
        

        #endregion


        #endregion
        
        

        
        

        public void Init(InteractableGridSystem interactableGridSystem,CD_Grid data = null)
        {
            if (data != null) { _data = data;}
            _interactableGridSystem = interactableGridSystem;
            _spinCoroutines = new Coroutine[interactableGridSystem.Dimensions.x];
            columnSpinStates = new SpinState[interactableGridSystem.Dimensions.x];
            currentSpinSpeeds = new float[interactableGridSystem.Dimensions.x];
            for (int i = 0; i < interactableGridSystem.Dimensions.x; i++)
            {
                columnSpinStates[i] = SpinState.Idle;
                currentSpinSpeeds[i] = _data.GridSpinData.currentSpinSpeed;
            }
        }
    

        private SpinState[] columnSpinStates;
        private float[] currentSpinSpeeds;

        private SpinState spinState = SpinState.Idle;
        public void StartSpin()
        {
            switch (GameManager.State)
            {
                case GameStates.SpinActive:
                case GameStates.GameStart:

                    GameManager.Instance.UpdateGameState(GameStates.Spinning);
                    StartCoroutine(SpinColumns());
                    break;
                case GameStates.Spinning:
                    GameManager.Instance.UpdateGameState(GameStates.SpinEnd);
                    StartCoroutine(DecelerateColumns());
                    break;
            }
        }

        IEnumerator DecelerateColumns()
        {
            for (int i = 0; i < Dimensions.x; i++)
            {
                columnSpinStates[i] = SpinState.Decelerating;

                yield return new WaitForSeconds(0.3f);
            }
            
        }

        IEnumerator SpinColumns()
        {
            for (int i = 0; i < Dimensions.x; i++)
            {
                currentSpinSpeeds[i] = _data.GridSpinData.currentSpinSpeed;
                columnSpinStates[i] = SpinState.Idle;
            }

            for (int i = 0; i < Dimensions.x; i++)
            {
                columnSpinStates[i] = SpinState.Spinning;
                SpinColumn(i);
                yield return new WaitForSeconds(0.15f);
            }
        }

        void SpinColumn(int columnIndex)
        {
            List<InteractableManager> column = _interactableGridSystem.GetColumn(columnIndex);

            _spinCoroutines[columnIndex] = StartCoroutine(SpinColumn(column, columnIndex));
        }

        IEnumerator SpinColumn(List<InteractableManager> interactablesOfColumn, int columnIndex)
        {
            AddExtraElementOnTop(interactablesOfColumn, columnIndex);

            List<InteractableManager> interactablesToRemove = new List<InteractableManager>();

            while (columnSpinStates[columnIndex] == SpinState.Spinning ||
                   columnSpinStates[columnIndex] == SpinState.Decelerating)
            {
                Dictionary<InteractableManager, Vector3> targetPositions = SetTargetPositions(interactablesOfColumn);

                yield return StartCoroutine(MoveInteractablesToTarget(interactablesOfColumn, targetPositions,
                    columnIndex));

                SetPositions(interactablesOfColumn, columnIndex, interactablesToRemove);

                RemoveOutGridInteractable(interactablesOfColumn, interactablesToRemove);

                if (columnSpinStates[columnIndex] != SpinState.Idle)
                {
                    AddExtraElementOnTop(interactablesOfColumn, columnIndex);
                }
            }
        }

        private void RemoveOutGridInteractable(List<InteractableManager> interactablesOfColumn,
            List<InteractableManager> interactablesToRemove)
        {
            foreach (var interactable in interactablesToRemove)
            {
                interactable.matrixPosition = new Vector2Int(-1, -1);
                interactablesOfColumn.Remove(interactable);
                GameSignals.Instance.onObjectReturnToPool?.Invoke(interactable);
            }

            interactablesToRemove.Clear();
        }

        private void SetPositions(List<InteractableManager> interactablesOfColumn, int columnIndex,
            List<InteractableManager> interactablesToRemove)
        {
            foreach (var interactable in interactablesOfColumn.ToList())
            {
                Vector2Int currentGridPosition = interactable.matrixPosition;

                if (currentGridPosition == new Vector2Int(columnIndex, 0))
                {
                    interactablesToRemove.Add(interactable);
                    _interactableGridSystem.RemoveItemFromGrid(currentGridPosition);
                    continue;
                }

                if (currentGridPosition != new Vector2Int(-1, -1))
                    _interactableGridSystem.RemoveItemFromGrid(currentGridPosition);

                // Update matrix position
                _interactableGridSystem.SetMatrixPosition(columnIndex, interactable, currentGridPosition);

                // Add interactable to new grid position
                if (_interactableGridSystem.IsInsideGrid(interactable.matrixPosition))
                {
                    _interactableGridSystem.PutItemOnGrid(interactable, interactable.matrixPosition);
                }
            }

            // After updating positions, check for matches and adjust spin state
            if (currentSpinSpeeds[columnIndex] <= _data.GridSpinData.spinSpeedBottomLimit)
            {
                List<InteractableManager> interactablesToCheck = interactablesOfColumn
                    .Where(x => !interactablesToRemove.Contains(x))
                    .ToList();

                if (_interactableGridSystem.AnyMatchWithLeftColumn(interactablesToCheck))
                {
                    // Match found with left column, continue spinning
                    Debug.Log($"Column {columnIndex}: Match found with left column. Continuing spin.");
                    columnSpinStates[columnIndex] = SpinState.Decelerating;
                }
                else
                {
                    if (columnIndex == 0)
                    {
                        Debug.Log($"Column {columnIndex}: Stopping spin.");
                        columnSpinStates[columnIndex] = SpinState.Idle;
                    }
                    else if (columnSpinStates[columnIndex - 1] == SpinState.Idle)
                    {
                        Debug.Log($"Column {columnIndex}: No Match, Stopping spin.");
                        columnSpinStates[columnIndex] = SpinState.Idle;
                        if (columnIndex == Dimensions.x - 1)
                        {
                            GameManager.Instance.UpdateGameState(GameStates.SpinActive);
                        }
                    }
                }

                if (columnSpinStates[columnIndex] == SpinState.Idle)
                {
                    _interactableGridSystem.SetWorldPositions(columnIndex, interactablesToCheck,transform);
                }
            }
        }
        
        
        IEnumerator MoveInteractablesToTarget(List<InteractableManager> interactablesOfColumn, Dictionary<InteractableManager, Vector3> targetPositions, int columnIndex)
        {
            bool allReachedTarget = false;
            while (!allReachedTarget)
            {
                allReachedTarget = true;
                foreach (var interactable in interactablesOfColumn)
                {
                    Vector3 targetPosition = targetPositions[interactable];
                    interactable.transform.position = Vector3.MoveTowards(interactable.transform.position, targetPosition, currentSpinSpeeds[columnIndex] * Time.deltaTime);

                    if (Vector3.Distance(interactable.transform.position, targetPosition) >= 0.01f)
                    {
                        allReachedTarget = false;
                    }

                    if (columnSpinStates[columnIndex] == SpinState.Decelerating)
                    {
                        currentSpinSpeeds[columnIndex] -= _data.GridSpinData.deceleration * Time.deltaTime;
                        if (currentSpinSpeeds[columnIndex] <= _data.GridSpinData.spinSpeedBottomLimit)
                        {
                            currentSpinSpeeds[columnIndex] = _data.GridSpinData.spinSpeedBottomLimit;
                        }
                    }
                }

                yield return null;
            }
        }
        
        private Dictionary<InteractableManager, Vector3> SetTargetPositions(List<InteractableManager> interactablesOfColumn)
        {
            Dictionary<InteractableManager, Vector3> targetPositions = new Dictionary<InteractableManager, Vector3>();

            foreach (var interactable in interactablesOfColumn)
            {
                if (interactable == null)
                {
                    Debug.LogWarning("Interactable is null in SetTargetPositions.");
                    continue;
                }
                Vector3 targetPosition = interactable.transform.position - new Vector3(0, GridSpacing, 0);
                targetPositions[interactable] = targetPosition;
            }

            return targetPositions;
        }
        
        private void AddExtraElementOnTop(List<InteractableManager> interactablesOfColumn, int columnIndex)
        {
            var extraElement = GameSignals.Instance.onGetInteractable.Invoke();

            if (extraElement == null)
            {
                Debug.LogError("Extra element is null. Pool may not be properly initialized.");
                return; // Handle this case appropriately
            }

            if (interactablesOfColumn.Count > 0 && interactablesOfColumn.First() != null && interactablesOfColumn.Last() != null)
            {
                // If the bottom and top element of the column are the same, they come together.
                // In order to avoid it, set the new element to the next id if there is a match.
                if (interactablesOfColumn.First().id == interactablesOfColumn.Last().id)
                {
                    GameSignals.Instance.onCycleInteractableType(extraElement);
                }
                else
                {
                    GameSignals.Instance.onAssignTypeToInteractable(extraElement, interactablesOfColumn.First().id);
                }
            }
            else
            {
                // Handle the case when 'interactablesOfColumn' is empty or has null entries
                GameSignals.Instance.onAssignRandomTypeToInteractable(extraElement);
            }
            extraElement.gameObject.SetActive(true);
            extraElement.transform.position = _interactableGridSystem.GridPositionToWorldPosition(new Vector2Int(columnIndex, Dimensions.y - 1),transform) + new Vector3(0, GridSpacing, 0);

            interactablesOfColumn.Add(extraElement);
        }
    }
}