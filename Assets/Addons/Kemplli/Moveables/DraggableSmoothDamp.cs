using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSmoothDamp : ManagedBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public MoveableBase moveable = null;

    private Camera mainCamera;
    public bool isDragging { get; private set; } = false;

    Vector3 offset;

    public DragState state { get; private set; } = DragState.Idle;

    public static DraggableSmoothDamp CurrentDraggable {  get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
        moveable.targetPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        moveable.enabled = true;
        StartDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDrag(eventData);
        moveable.enabled = false;
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
        CurrentDraggable = this;

        state = DragState.Dragging;
    }
     
    private void StopDrag(PointerEventData eventData)
    {
        if (CurrentDraggable != null)
        {
            isDragging = false;
            moveable.targetPosition = CurrentDraggable.transform.position;
            CurrentDraggable = null;

            state = DragState.Idle;
        }
    }

    private void Drag(PointerEventData eventData)
    {
        if (CurrentDraggable != null)
        {
            Vector3 cursorPoint = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
            Vector3 cursorPosition = mainCamera.ScreenToWorldPoint(cursorPoint) + offset;
            cursorPosition.z = transform.position.z;

            moveable.targetPosition = cursorPosition;
        }
    }
}

public enum DragState
{
    Idle,
    Dragging
}
