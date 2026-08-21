using UnityEngine;

public class TimedBehaviour : ManagedBehaviour
{
    [Header("Timer settings")]
    public float frequency;

    public bool UnscaledTime;

    private float timer = 0;

    public override void ManagedUpdate()
    {
        timer += UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timer >= frequency)
        {
            OnTimerReached();
            ResetTimer();
        }
    }

    protected virtual void OnTimerReached() { }

    protected void ResetTimer()
    {
        timer = 0;
    }

    protected void SkipToEndTimer()
    {
        timer = frequency;
    }
}
