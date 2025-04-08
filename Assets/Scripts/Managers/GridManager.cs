using System;
using System.Collections.Generic;
using Commands.Grid;
using Controllers.Grid;
using Data.UnityObjects;
using Enums;
using Extensions;
using Grid;
using Signals;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoSingleton<GridManager>
    {
        #region Self Variables

        #region Private Variables
        
        private InteractableGridSystem interactableGridSystem;
        private OnSwapCommand onSwapCommand;
        private FillGridCommand _fillGridCommand;
        private PlaceGridCommand _placeGridCommand;
        
        private CD_Grid _gridData;
        private Vector2Int Dimensions => interactableGridSystem.Dimensions;
        private int minimumAmountOfEachType = 3;
        
        #endregion
        
        #region Serialized Variables
        
        [SerializeField] private GridSpriteController spriteController;
        [SerializeField] private GridMaskController gridMaskController;
        [SerializeField] GridSpinController gridSpinner;

        
        #endregion
        
        #endregion


        private void Awake()
        {
            GetData();    
            Init();
        }

        private void GetData()
        {
            _gridData = Resources.Load<CD_Grid>("Data/Grid/CD_Grid");
        }

        private void Init()
        {
            interactableGridSystem = new InteractableGridSystem();
            interactableGridSystem.CreateGrid();
            gridSpinner.Init(interactableGridSystem,_gridData);
            spriteController.SetRendererData(Dimensions, _gridData);
            gridMaskController.SetMaskControllerData(Dimensions, _gridData);

            _placeGridCommand = new PlaceGridCommand(_gridData,transform,() => Dimensions);
            _fillGridCommand = new FillGridCommand(interactableGridSystem);
            onSwapCommand = new OnSwapCommand(interactableGridSystem,this);

        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameSignals.Instance.onGameInitialize += BuildGrid;
            GameSignals.Instance.onGetGrid += () => interactableGridSystem;
            GameSignals.Instance.onSpinButtonPressed += gridSpinner.StartSpin;
            GameSignals.Instance.onSwap += onSwapCommand.Execute;
            GameSignals.Instance.onRestart += OnRestart;
        }

        private void OnRestart()
        {
            ClearGrid();
            interactableGridSystem.CreateGrid();
            spriteController.SetRendererData(Dimensions);
            gridMaskController.SetMaskControllerData(Dimensions);
            gridMaskController.SetSpriteMask();
            spriteController.SetGridBackGroundSprite();
            
            
            List<InteractableManager> interactablesToFill =
                GameSignals.Instance.onGetStartInteractables?.Invoke(minimumAmountOfEachType, interactableGridSystem.GridCapacity);
            
            _placeGridCommand.Execute();
            _fillGridCommand.Execute(interactablesToFill,interactableGridSystem.Dimensions,transform);
            gridSpinner.Init(interactableGridSystem);
                
        }

        private void BuildGrid()
        {
            spriteController.SetGridBackGroundSprite();
            gridMaskController.SetSpriteMask();
            GameSignals.Instance.onGridInitialize(Dimensions);
            List<InteractableManager> interactablesToFill =
                GameSignals.Instance.onGetStartInteractables?.Invoke(minimumAmountOfEachType, interactableGridSystem.GridCapacity);
            _placeGridCommand.Execute();
            _fillGridCommand.Execute(interactablesToFill,interactableGridSystem.Dimensions,transform);
        }
        
        private void UnSubscribeEvents()
        {
            GameSignals.Instance.onGameInitialize -= BuildGrid;
            GameSignals.Instance.onGetGrid -= () => interactableGridSystem;
            GameSignals.Instance.onSpinButtonPressed += gridSpinner.StartSpin;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        
        private void ClearGrid()
        {
            gridSpinner.StopAllCoroutines();
            StopAllCoroutines();
            interactableGridSystem.ClearGrid();
            GameSignals.Instance.onClearPool?.Invoke();
        }
    }
}