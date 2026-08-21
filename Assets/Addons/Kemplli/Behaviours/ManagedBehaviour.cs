using System;
using UnityEngine;

public class ManagedBehaviour : MonoBehaviour
{
    public virtual void ManagedUpdate() { }
    public virtual void PausedUpdate() { }
    public virtual void ManagedFixedUpdate() { }
    public virtual void ManagedLateUpdate() { }
    protected virtual void ManagedInitialize() { }
    protected virtual void ManagedStart() { }
    protected virtual void ManagedOnCollisionEnter(Collision2D collision) { }
    protected virtual void ManagedOnCollisionStay(Collision2D collision) { }
    protected virtual void ManagedOnCollisionExit(Collision2D collision) { }
    protected virtual void ManagedOnTriggerEnter(Collider2D collision) { }
    protected virtual void ManagedOnTriggerExit(Collider2D collision) { }
    protected virtual void ManagedOnTriggerStay(Collider2D collision) { }


    public void Update()
    {
        if (!G.IsPaused)
            ManagedUpdate();
    }

    public void LateUpdate()
    {
        if (!G.IsPaused)
            ManagedLateUpdate();
    }

    public void FixedUpdate()
    {
        if (!G.IsPaused)
            ManagedFixedUpdate();
    }

    public void Awake()
    {
        ManagedInitialize();
    }

    public void Start()
    {
        ManagedStart();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!G.IsPaused)
            ManagedOnCollisionEnter(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!G.IsPaused)
            ManagedOnTriggerEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!G.IsPaused && this.enabled)
            ManagedOnTriggerExit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!G.IsPaused)
            ManagedOnTriggerStay(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!G.IsPaused)
            ManagedOnCollisionStay(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!G.IsPaused)
            ManagedOnCollisionExit(collision);
    }
}
