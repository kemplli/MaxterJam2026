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

    private SortingGroup sortingGroup;

    private Collider2D collider;

    private Vector3 startScale;
    public bool sorting;

    public DragState state { get; private set; } = DragState.Idle;

    private void Start()
    {
        mainCamera = Camera.main;
        sortingGroup = GetComponent<SortingGroup>();
        collider = GetComponent<Collider2D>();
        startScale = transform.localScale;
        moveable.targetPosition = transform.parent != null ? transform.localPosition : transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //   moveable.enabled = true;
        StartDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDrag(eventData);
        PointerDown();
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
        if(collider.enabled) collider.enabled = false;

        state = DragState.Dragging;
    }
     
    private void StopDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            moveable.targetPosition = transform.position;
            if (!collider.enabled) collider.enabled = true;

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
    }

    protected virtual void OnRelease() { }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(startScale * 1.2f, 0.25f);
        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            transform.DOKill();
            transform.DOScale(startScale, 0.25f);
        }
        PointerExit();
    }

    protected virtual void PointerEnter() { }
    protected virtual void PointerExit() { }
    protected virtual void PointerDown() { }
}

public enum DragState
{
    Idle,
    Dragging
}
