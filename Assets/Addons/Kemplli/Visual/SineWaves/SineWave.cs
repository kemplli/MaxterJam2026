using UnityEngine;

public abstract class SineWave : MonoBehaviour
{
    public Vector3 magnitude = default;
    public float speed = default;
    public float timeOffset = default;

    public bool randomizeTimeOffset = default;

    private Vector3 originalMagnitude;

    public bool unscaledTime;

    private void Awake()
    {
        originalMagnitude = GetMagnitude();
        if (randomizeTimeOffset) timeOffset = Random.Range(0f, 100f);
    }

    //public override void ManagedUpdate()
    //{
    //    float sine = (Mathf.Sin(Time.time * speed + timeOffset)) + 0.5f;
    //    float x = originalMagnitude.x + (sine * magnitude.x);
    //    float y = originalMagnitude.y + (sine * magnitude.y);
    //    float z = originalMagnitude.z + (sine * magnitude.z);
    //    ApplyTransformation(new Vector3(x, y, z));
    //}

    private void Update()
    {
        float sine = (Mathf.Sin(unscaledTime == true ? Time.unscaledTime * speed + timeOffset : Time.time * speed + timeOffset)) + 0.5f;
        float x = originalMagnitude.x + (sine * magnitude.x);
        float y = originalMagnitude.y + (sine * magnitude.y);
        float z = originalMagnitude.z + (sine * magnitude.z);
        ApplyTransformation(new Vector3(x, y, z));
    }


    protected abstract Vector3 GetMagnitude();
    protected abstract void ApplyTransformation(Vector3 value);
}
