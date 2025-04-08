using System;
using System.Collections.Generic;
using Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Signals
{
    public class GameSignals: MonoSingleton<GameSignals>
    {
        public UnityAction onGameInitialize = delegate { };
        public UnityAction<Vector2Int> onGridInitialize = delegate { };
        
        public UnityAction onReadyToSpin = delegate { };
        public UnityAction onGamePlay = delegate { };
        public UnityAction onNewGamePlay;
        public UnityAction onSpin = delegate { };
        public UnityAction onStopSpin = delegate { };
        public UnityAction onMatch = delegate { };
        
        //Pool signals
        public Func<int,int,List<Interactable>> onGetStartInteractables = delegate{ return null; };
        public Func<Interactable> onGetInteractable = delegate { return null; };
        public UnityAction<Interactable> onCycleInteractableType = delegate { };
        public UnityAction<Interactable,int> onAssignTypeToInteractable = delegate { };
        public UnityAction<Interactable> onAssignRandomTypeToInteractable = delegate { };
        public UnityAction<Interactable> onObjectReturnToPool = delegate { };
        public UnityAction onClearPool = delegate { };
        
        //Input
        public Func<InteractableGridSystem> onGetGrid = delegate { return null; };
        
        //UI
        public UnityAction onSpinButtonPressed = delegate { };
        public UnityAction onContinueButtonPressed = delegate { };
        
        //Grid
        public UnityAction onStartSpin = delegate { };
    }
}