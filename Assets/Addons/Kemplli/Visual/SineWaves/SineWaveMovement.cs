using UnityEngine;

public class SineWaveMovement : SineWave
{
    [Header("Custom Magnitude")]
    public Vector3 _originalMagnitude;
    public bool autoMagnitude;

    protected override void ApplyTransformation(Vector3 value)
    {
        transform.localPosition = value;
    }

    protected override Vector3 GetMagnitude()
    {
        if (!autoMagnitude)
            return transform.localPosition;
        else
            return _originalMagnitude;
    }
}
