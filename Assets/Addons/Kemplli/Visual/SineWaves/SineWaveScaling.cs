using UnityEngine;

public class SineWaveScaling : SineWave
{
    [Header("Custom Magnitude")]
    public Vector3 _originalMagnitude;
    public bool autoMagnitude;

    protected override void ApplyTransformation(Vector3 value)
    {
        transform.localScale = value;
    }

    protected override Vector3 GetMagnitude()
    {
        if(!autoMagnitude)
        return transform.localScale;
        else
            return _originalMagnitude;
    }
}
