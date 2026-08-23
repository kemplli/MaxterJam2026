using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GlobalVolumeController : MonoBehaviour
{
    public Volume globalVolume;

    [NonSerialized]
    public Vignette vignette;

    void Start()
    {
        G.gVolCon = this;

        if (globalVolume.profile.TryGet(out Vignette Vignette))
        {
            vignette = Vignette;
        }
    }

    public void SetGlobalWeight(float weight)
    {
        globalVolume.weight = weight;
    }

    private IEnumerator VignetteFlashCoroutine(float startValue, float endValue,Color color)
    {
        vignette.color.value = color;
        while (vignette.intensity.value < endValue)
        {
            vignette.intensity.value += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.05f);

        while (vignette.intensity.value > startValue)
        {
            vignette.intensity.value -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }




    public void VignetteFlash(float startValue, float endValue,Color color)
    {
        StopAllCoroutines();
        StartCoroutine(VignetteFlashCoroutine(startValue, endValue,color));
    }
}
