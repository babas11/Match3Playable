using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    [SerializeField]
    public UnityEvent onButtonDown;

    [SerializeField]
    public UnityEvent onButtonUp;

    SpriteRenderer spriteRenderer;
    CircleCollider2D collider;



    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }


    public void SetUpButton(Vector3 scale, Sprite sprite/* , float distanceFromScreenBottom */)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 1;
        //PositionButtonOnCenterOfTheScreen(distanceFromScreenBottom);
        transform.localScale = scale;
        collider = gameObject.AddComponent<CircleCollider2D>();

    }
    

    public void OnPointerDown(PointerEventData eventData)
    {
        onButtonDown?.Invoke();
        transform.DOScale(new Vector3(1.5f,1.5f,1),0.1f).SetLoops(2,LoopType.Yoyo);
        print("DDOOOWWNNN");
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onButtonUp?.Invoke();
    }
}