using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enums;
using Signals;
using UnityEngine;

public class InteractableGridSystem : GridSystem<Interactable>
{
    [SerializeField]
    //public InteractablePool interactablePool;

    Coroutine[] spinCoroutines;

    [Tooltip("Distance between each grid cell")]
    [SerializeField]
    private float gridSpacing = 0.6f;
    public float GridSpacing => gridSpacing;


    [SerializeField]
    private float currentSpinSpeed = 10f;

    private float spinSpeedBottomLimit = 10f;
    private float deceleration = 1f;
    private int minimumAmountOfEachType = 3;

    

    private SpinState[] columnSpinStates;
    private float[] currentSpinSpeeds;

    private SpinState spinState = SpinState.Idle;

   

    /*public void ClearGrid()
    {
        StopAllCoroutines();
        ClerGrid();
        GameSignals.Instance.onClearPool?.Invoke();
    }*/

    /*private int CheckDirection(Vector2Int direction, Interactable interactable)
    {
        int matches = 0;
        Vector2Int position = direction + interactable.matrixPosition;

        while (IsInsideGrid(position) &&
               !IsEmpty(position) &&
                   GetItemOnGrid(position).id == interactable.id)
        {
            matches++;
            position += direction;

        }

        return matches;
    }*/
    
    /*private bool IsThereAnyMatch(Interactable interactableToCheck, bool checkForHorizontal = true, bool checkForVertical = true)
    {
        if (checkForHorizontal)
        {
            int matchinInterablesHorizontal = 0;
            //Look For Left
            Vector2Int leftDirection = Vector2Int.left;
            matchinInterablesHorizontal += CheckDirection(leftDirection, interactableToCheck);

            //Look for right
            Vector2Int rightDirection = Vector2Int.right;
            matchinInterablesHorizontal += CheckDirection(rightDirection, interactableToCheck);

            if (matchinInterablesHorizontal > 0) { return true; }

        }

        if (checkForVertical)
        {
            int matchingInteractablesVertical = 0;
            //Look For Up
            Vector2Int upDirection = Vector2Int.up;
            matchingInteractablesVertical += CheckDirection(upDirection, interactableToCheck);

            //Look for Down
            Vector2Int downDirection = Vector2Int.down;
            matchingInteractablesVertical += CheckDirection(downDirection, interactableToCheck);

            if (matchingInteractablesVertical > 0) { return true; }

        }

        return false;

    }*/
    
    public void SetWorldPositions(int columnIndex, List<Interactable> interactables,Transform transform)
    {
        foreach (var interactable in interactables)
        {
            interactable.transform.position = GridPositionToWorldPosition(interactable.matrixPosition,transform);

        }
    }

    public void SetMatrixPosition(int columnIndex, Interactable interactable, Vector2Int currentGridPosition)
    {
        if (currentGridPosition == new Vector2Int(-1, -1))
        {
            interactable.matrixPosition = new Vector2Int(columnIndex, Dimensions.y - 1);

        }
        else
        {
            interactable.matrixPosition = new Vector2Int(currentGridPosition.x, currentGridPosition.y - 1);

        }
    }
    public bool AnyMatchWithLeftColumn(List<Interactable> interactablesToCheck)
    {
        foreach (var interactableToCheck in interactablesToCheck)
        {
            Vector2Int onLeftPosition = interactableToCheck.matrixPosition + Vector2Int.left;

            if (!IsInsideGrid(onLeftPosition))
            {
                return false;
            }

            if (GetItemOnGrid(onLeftPosition).id == interactableToCheck.id)
            {
                return true;
            }
        }
        return false;
    }
    
    
    #region Swap Mechanics

    /*public IEnumerator Swap(Interactable[] interactables)
    {
        Interactable[] copies = new Interactable[2];
        copies[0] = interactables[0];
        copies[1] = interactables[1];

        ChangeItemsAt(copies[0].matrixPosition, copies[1].matrixPosition);

        Vector2Int temp = copies[0].matrixPosition;
        copies[0].matrixPosition = copies[1].matrixPosition;
        copies[1].matrixPosition = temp;

        List<Interactable> interactable0Matches;
        List<Interactable> interactable1Matches;

        interactable0Matches = GetMatches(copies[0]);
        interactable1Matches = GetMatches(copies[1]);

        if (interactable0Matches != null || interactable1Matches != null)
        {
            GameManager.Instance.UpdateGameState(GameStates.LastMove);
        }


        yield return StartCoroutine(AnimateSwap(copies));


        if (interactable0Matches != null || interactable1Matches != null)
        {
            GameManager.Instance.UpdateGameState(GameStates.Won);
        }

    }*/

    private List<Interactable> GetMatchesInDirection(Interactable toMatch, Vector2Int direction)
    {
        List<Interactable> interactables = new List<Interactable>();
        Vector2Int position = toMatch.matrixPosition + direction;
        Interactable next;

        while (IsInsideGrid(position) && !IsEmpty(position))
        {
            next = GetItemOnGrid(position);

            if (next.id == toMatch.id && next.Idle)
            {
                interactables.Add(GetItemOnGrid(position));
                position += direction;
            }
            else break;

        }

        return interactables;
    }



    private List<Interactable> GetMatches(Interactable interactable)
    {
        if (interactable == null || !IsInsideGrid(interactable.matrixPosition) || IsEmpty(interactable.matrixPosition)) return null;


        List<Interactable> matchesList = new List<Interactable>();
        matchesList.Add(interactable);

        List<Interactable> horizontalMatch, verticalMatch;

        horizontalMatch = GetMatchesInDirection(interactable, Vector2Int.left);
        horizontalMatch.AddRange(GetMatchesInDirection(interactable, Vector2Int.right));
        if (horizontalMatch.Count > 1)
        {
            matchesList.AddRange(horizontalMatch);
        }

        verticalMatch = GetMatchesInDirection(interactable, Vector2Int.up);
        verticalMatch.AddRange(GetMatchesInDirection(interactable, Vector2Int.down));
        if (verticalMatch.Count > 1)
        {
            matchesList.AddRange(verticalMatch);
        }

        if (matchesList.Count < 3) return null;

        return matchesList;



    }

    IEnumerator AnimateSwap(Interactable[] interactables)
    {
        Interactable interactable0 = interactables[0];
        Interactable interactable1 = interactables[1];

        Vector3[] targetPositions = new Vector3[2];
        targetPositions[0] = interactable1.transform.position;
        targetPositions[1] = interactable0.transform.position;

        Vector3 scale = new Vector3(1.2f, 1.2f, 1f);

        // create tweens for scaling up
        Tween scaleUpTween0 = interactable0.transform.DOScale(scale, 0.5f)
            .SetEase(Ease.InQuad)
            .OnStart(() => interactable0.Idle = false);

        Tween scaleUpTween1 = interactable1.transform.DOScale(scale, 0.5f)
            .SetEase(Ease.InQuad)
            .OnStart(() => interactable1.Idle = false);

        // create tweens for moving
        Tween moveTween0 = interactable0.transform.DOMove(targetPositions[0], 1f)
            .SetEase(Ease.InQuad);

        Tween moveTween1 = interactable1.transform.DOMove(targetPositions[1], 1f)
            .SetEase(Ease.InQuad);

        // Create Tweens for scaling down
        Tween scaleDownTween0 = interactable0.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => interactable0.Idle = true);

        Tween scaleDownTween1 = interactable1.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.InQuad)
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





    #endregion


}





