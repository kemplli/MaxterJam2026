using DG.Tweening;
using UnityEngine;

public class Rotate360 : ManagedBehaviour
{
    public float turnoverDuration;

    public bool paused;

    public bool unscaledTime;

    private void OnEnable()
    {
        Rotate();
    }

    private void Rotate()
    {
        var endValue = new Vector3(0f, 0f, 1f + transform.rotation.eulerAngles.z + 360f);
        transform.DORotate(endValue, turnoverDuration, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart).SetUpdate(unscaledTime);
    }

    public void SetPaused(bool paused)
    {
        if (paused)
            transform.DOKill();
        else
            Rotate();

        this.paused = paused;
    }

    public void Reload() { SetPaused(true); SetPaused(false); }

    private void OnDisable()
    {
        SetPaused(true);
    }
}
