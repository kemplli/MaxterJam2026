using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DraggableSmoothDamp : ManagedBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public MoveableBase moveable = null;

    private Camera mainCamera;
    public bool isDragging { get; private set; } = false;

    Vector3 offset;

    protected SortingGroup sortingGroup;

    private Collider2D collider;

    private Vector3 startScale;
    public bool sorting;

    public string defaultSortingLayer;

    public DragState state { get; private set; } = DragState.Idle;

    [SerializeField]
    private bool selecting = true;

    private void Start()
    {
        mainCamera = Camera.main;
        sortingGroup = GetComponent<SortingGroup>();
        collider = GetComponent<Collider2D>();
        startScale = transform.localScale;
        moveable.targetPosition = transform.parent != null ? transform.localPosition : transform.position;
        defaultSortingLayer = sortingGroup.sortingLayerName;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //   moveable.enabled = true;
        StartDrag(eventData);
        PointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDrag(eventData);
        PointerUp();
        //moveable.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag(eventData);
    }

    private void StartDrag(PointerEventData eventData)
    {
        moveable.targetPosition = transform.position;
        isDragging = true;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, screenPosition.z));
        if(collider.enabled && selecting) collider.enabled = false;
        sortingGroup.sortingLayerName = "Dragging";

        state = DragState.Dragging;
    }
     
    private void StopDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            moveable.targetPosition = transform.position;
            if (!collider.enabled && selecting) collider.enabled = true;
            sortingGroup.sortingLayerName = defaultSortingLayer;

            state = DragState.Idle;
            OnRelease();
          //  transform.DOKill();
          //  transform.DOScale(startScale, 0.25f);
        }
    }

    private void Drag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 cursorPoint = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
            Vector3 cursorPosition = (Vector3)GameMath.MousePos2D() + offset;
            cursorPosition.z = transform.position.z;

            moveable.targetPosition = cursorPosition;
        }
    }

    public override void ManagedUpdate()
    {
        if(sorting)
        sortingGroup.sortingOrder = isDragging ? 1000 : 0;

        sortingGroup.sortingLayerName = defaultSortingLayer;
    }

    protected virtual void OnRelease() { }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selecting)
        {
            transform.DOKill();
            transform.DOScale(startScale * 1.2f, 0.25f);
        }
        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            if (selecting)
            {
                transform.DOKill();
                transform.DOScale(startScale, 0.25f);
            }
        }
        PointerExit();
    }

    protected virtual void PointerEnter() { }
    protected virtual void PointerExit() { }
    protected virtual void PointerDown() { }
    protected virtual void PointerUp() { }
}

public enum DragState
{
    Idle,
    Dragging
}

