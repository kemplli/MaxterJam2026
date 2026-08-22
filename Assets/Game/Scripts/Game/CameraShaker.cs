using System;
using UnityEngine;

public class CameraShaker : ManagedBehaviour
{
    [NonSerialized]
    public float shakeAmount = 0f;

    private Vector3 _lastOffset = Vector3.zero;

    protected override void ManagedInitialize()
    {
        G.cameraShaker = this;
    }

    public override void ManagedLateUpdate()
    {
        if (shakeAmount <= 0)
        {
            if (_lastOffset != Vector3.zero)
            {
                transform.localPosition -= _lastOffset;
                _lastOffset = Vector3.zero;
            }
            return;
        }

        Vector3 cleanLocalPos = transform.localPosition - _lastOffset;

        Vector3 newOffset = UnityEngine.Random.insideUnitSphere * shakeAmount;
        newOffset.z = 0;

        transform.localPosition = cleanLocalPos + newOffset;

        _lastOffset = newOffset;
    }

}