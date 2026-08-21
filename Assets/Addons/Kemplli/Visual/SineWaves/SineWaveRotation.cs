using UnityEngine;

public class SineWaveRotation : SineWave
{
    [Header("Custom Magnitude")]
    public Vector3 _originalMagnitude;
    public bool autoMagnitude;

    protected override void ApplyTransformation(Vector3 value)
    {
        transform.localEulerAngles = value;
    }

    protected override Vector3 GetMagnitude()
    {
        if(!autoMagnitude)
        return transform.localEulerAngles;
        else
            return _originalMagnitude;
    }
}
