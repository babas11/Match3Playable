using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using Grid;
using Managers;
using UnityEngine;

namespace Commands.Grid
{
    public class OnSwapCommand
    {
        private InteractableGridSystem _interactableGridSystem;
        private GridManager _manager;
        
        public OnSwapCommand(InteractableGridSystem interactableGridSystem, GridManager manager)
        {
            _interactableGridSystem = interactableGridSystem;
            _manager = manager;
        }

        internal void Execute(InteractableManager[] selectedInteractables)
        {
            _manager.StartCoroutine(Swap(selectedInteractables));
        }
        
        private IEnumerator Swap(InteractableManager[] interactables)
        {
            InteractableManager[] copies = new InteractableManager[2];
            copies[0] = interactables[0];
            copies[1] = interactables[1];

            _interactableGridSystem.ChangeItemsAt(copies[0].matrixPosition, copies[1].matrixPosition);

            Vector2Int temp = copies[0].matrixPosition;
            copies[0].matrixPosition = copies[1].matrixPosition;
            copies[1].matrixPosition = temp;

            List<InteractableManager> interactable0Matches;
            List<InteractableManager> interactable1Matches;

            interactable0Matches = _interactableGridSystem.GetMatches(copies[0]);
            interactable1Matches = _interactableGridSystem.GetMatches(copies[1]);

            if (interactable0Matches != null || interactable1Matches != null)
            {
                GameManager.Instance.UpdateGameState(GameStates.LastMove);
            }
        
            yield return _manager.StartCoroutine(AnimateSwap(copies));
        
            if (interactable0Matches != null || interactable1Matches != null)
            {
                GameManager.Instance.UpdateGameState(GameStates.Won);
            }

        }
         IEnumerator AnimateSwap(InteractableManager[] interactables)
            {
                InteractableManager interactable0 = interactables[0];
                InteractableManager interactable1 = interactables[1];
        
                Vector3[] targetPositions = new Vector3[2];
                targetPositions[0] = interactable1.transform.position;
                targetPositions[1] = interactable0.transform.position;
        
                Vector3 scale = new Vector3(1.2f, 1.2f, 1f);
        
                // create tweens for scaling up
                Tween scaleUpTween0 = interactable0.transform.DOScale(scale, 0.2f)
                    .SetEase(Ease.InQuad)
                    .OnStart(() => interactable0.Idle = false);
        
                Tween scaleUpTween1 = interactable1.transform.DOScale(scale, 0.2f)
                    .SetEase(Ease.InQuad)
                    .OnStart(() => interactable1.Idle = false);
        
                // create tweens for moving
                Tween moveTween0 = interactable0.transform.DOMove(targetPositions[0], 0.2f)
                    .SetEase(Ease.InQuad);
        
                Tween moveTween1 = interactable1.transform.DOMove(targetPositions[1], 0.2f)
                    .SetEase(Ease.InQuad);
        
                // Create Tweens for scaling down
                Tween scaleDownTween0 = interactable0.transform.DOScale(1f, 0.2f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => interactable0.Idle = true);
        
                Tween scaleDownTween1 = interactable1.transform.DOScale(1f, 0.2f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => interactable1.Idle = true);
        
                // Create main sequence
                Sequence mainSequence = DOTween.Sequence();
        
                // Add scaling up tweens (sequence0)
                mainSequence.Append(scaleUpTween0);
                mainSequence.Join(scaleUpTween1);
        
                // Add moving tweens (sequence1)
                mainSequence.Append(moveTween0);
                mainSequence.Join(moveTween1);
        
                // Add scaling down tweens
                mainSequence.Append(scaleDownTween0);
                mainSequence.Join(scaleDownTween1);
        
                // Wait for the sequence to complete
                yield return mainSequence.WaitForCompletion();
            }
    }
}