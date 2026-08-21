using UnityEngine;
using DG.Tweening;
using System;

public class Feel
{
    private Transform camera;

    public Feel()
    {
    }

    public void ShakeHard()
    {
        TryInitialize();
        camera.DOShakePosition(0.3f, 2f).OnComplete(() => camera.DOMove(new Vector3(0,0,-10),0.05f));
    }

    private void TryInitialize()
    {
        if (camera == null)
            camera = Camera.main.transform;
    }
}
