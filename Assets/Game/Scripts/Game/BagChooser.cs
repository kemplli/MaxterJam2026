using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BagChooser : ManagedBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float followSmoothTime = 0.05f; 

    [SerializeField] private float minXBound = -5f;          
    [SerializeField] private float maxXBound = -2.8f;        

    public System.Action OnLeftBoundaryHit;  
    public System.Action OnBoundaryReleased; 

    private float xVelocity;
    private bool isHolding;
    private bool wasLeftHitTriggered;

    private void Update()
    {
        if (!isHolding) return;

        var mousePosX = GameMath.MousePos2D().x;

        float targetX = Mathf.Clamp(mousePosX, minXBound, maxXBound);

        float smoothX = Mathf.SmoothDamp(transform.position.x, targetX, ref xVelocity, followSmoothTime);
        transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);

        if (smoothX <= minXBound + 0.01f)
        {
            if (!wasLeftHitTriggered)
            {
                wasLeftHitTriggered = true;
                OnLeftBoundaryHit?.Invoke();
            }
        }
        else
        {
            if (wasLeftHitTriggered)
            {
                wasLeftHitTriggered = false;
                OnBoundaryReleased?.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;

        G.audio.PlaySound("desk", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f), repetition: new AudioParams.Repetition(0.05f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        wasLeftHitTriggered = false;
    }

    protected override void ManagedOnTriggerExit(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Sack>(out Sack sack))
        {
            sack.transform.parent = null;
            sack.defaultSortingLayer = "Default";
        }
    }
}
