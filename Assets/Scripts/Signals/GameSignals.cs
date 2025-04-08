using System;
using System.Collections.Generic;
using Extensions;
using Grid;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Signals
{
    public class GameSignals: MonoSingleton<GameSignals>
    {
        //Game States
        public UnityAction onGameInitialize = delegate { };
        public UnityAction onReadyToSpin = delegate { };
        public UnityAction onSpinActive = delegate { };
        public UnityAction onsSpinning = delegate { };
        public UnityAction onMatchingActive = delegate { };
        public UnityAction onWin = delegate { };
        public UnityAction onRestart = delegate { };

        
        //Pool signals
        public Func<int,int,List<InteractableManager>> onGetStartInteractables = delegate{ return null; };
        public Func<InteractableManager> onGetInteractable = delegate { return null; };
        public UnityAction<InteractableManager> onCycleInteractableType = delegate { };
        public UnityAction<InteractableManager,int> onAssignTypeToInteractable = delegate { };
        public UnityAction<InteractableManager> onAssignRandomTypeToInteractable = delegate { };
        public UnityAction<InteractableManager> onObjectReturnToPool = delegate { };
        public UnityAction onClearPool = delegate { };
        
        //Input
        public Func<InteractableGridSystem> onGetGrid = delegate { return null; };
        public UnityAction<InteractableManager[]> onSwap = delegate { };
        
        //UI
        public UnityAction onSpinButtonPressed = delegate { };
        public UnityAction onContinueButtonPressed = delegate { };
        
        //Grid
        public UnityAction<Vector2Int> onGridInitialize = delegate { };
        public UnityAction onSpinEnds = delegate { };
    }
}