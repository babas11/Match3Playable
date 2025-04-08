using System;
using System.Collections.Generic;
using Commands.Grid;
using Controllers.Grid;
using Data.UnityObjects;
using Enums;
using Extensions;
using Signals;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoSingleton<GridManager>
    {
        #region Self Variables

        #region Private Variables
        
        InteractableGridSystem interactableGridSystem;
        [SerializeField] GridSpinController gridSpinner;
        FillGridCommand _fillGridCommand;
        private PlaceGridCommand _placeGridCommand;

        [SerializeField] private GridSpriteController spriteController;
        [SerializeField] private GridMaskController gridMaskController;
        private CD_Grid _gridData;
        [SerializeField]
        private Vector2Int Dimensions => interactableGridSystem.Dimensions;


        Coroutine[] spinCoroutines;

        [Tooltip("Distance between each grid cell")]
        [SerializeField]
        private float gridSpacing = 0.6f;
        public float GridSpacing => gridSpacing;
        
        private int minimumAmountOfEachType = 3;

        
        #endregion
        
        #endregion


        private void Awake()
        {
            GetData();
            Init();
            print(Dimensions);
        }

        private void GetData()
        {
            _gridData = Resources.Load<CD_Grid>("Data/Grid/CD_Grid");
        }

        private void Init()
        {
            interactableGridSystem = new InteractableGridSystem();
            interactableGridSystem.CreateGrid();
            gridSpinner.Init(interactableGridSystem);
            spriteController.SetRendererData(_gridData,Dimensions);
            gridMaskController.SetMaskControllerData(_gridData,Dimensions);

            _placeGridCommand = new PlaceGridCommand(_gridData,transform,Dimensions);
            _fillGridCommand = new FillGridCommand(interactableGridSystem);
            
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
        }

        private void BuildGrid()
        {
            spriteController.SetGridBackGroundSprite();
            gridMaskController.SetSpriteMask();
            GameSignals.Instance.onGridInitialize(Dimensions);
            List<Interactable> interactablesToFill =
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
            interactableGridSystem.ClearGrid();
            GameSignals.Instance.onClearPool?.Invoke();
        }
    }
}