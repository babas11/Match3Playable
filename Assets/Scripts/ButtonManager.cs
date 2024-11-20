using System.Diagnostics;
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
    Color defaultColor;
    Color inactiveColor;
    
    CircleCollider2D collider;

    Vector3 buttonAnimationScale = new Vector3(1, 1, 1);
    bool animateButton;
    float animationDuration = 0.1f;

    bool isActive = true;



    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Setting buttons alpha channels according to being enable
        defaultColor = new Color(1,1,1,1);
        inactiveColor = new Color(1,1,1,0.6f);

    }

    public void SetButtonActive(bool active)
    {
        isActive = active;
        spriteRenderer.color = isActive ? defaultColor : inactiveColor;
    }


    public void SetUpButton(Vector3 scale, Sprite sprite, int sortingOrder = 1, bool animateButton = false)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        transform.localScale = scale;
        collider = gameObject.AddComponent<CircleCollider2D>();
        this.animateButton = animateButton;

    }
    public void SetUpButton(Vector3 position, Vector3 scale, Sprite sprite, int sordingOrder = 1, bool animateButton = false, bool isActive = true)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sordingOrder;
        transform.position = position;
        transform.localScale = scale;
        collider = gameObject.AddComponent<CircleCollider2D>();
        this.animateButton = animateButton;
        gameObject.SetActive(isActive);
    }
    public void SetButtonAnimation(Vector3 maxScale, float duration = 0.1f)
    {
        buttonAnimationScale = maxScale;
        animationDuration = 0.1f;

    }

    public void ChangeButtonImage(Sprite toSprite)
    {
        spriteRenderer.sprite = toSprite;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (isActive)
        {
            onButtonDown?.Invoke();
            if (animateButton)
            {
                transform.DOScale(buttonAnimationScale, animationDuration).SetLoops(2, LoopType.Yoyo);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isActive)
        {
            onButtonUp?.Invoke();
        }

    }
}