using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class Transition : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image image;

    private Material material;

    [SerializeField]
    private string sound;

    [SerializeField]
    private bool withMaterial;

    [SerializeField]
    private float transitionSpeed;

    [SerializeField]
    private bool inUI;

    private void Awake()
    {
        G.transition = this;

        if (!inUI)
            spriteRenderer = GetComponent<SpriteRenderer>();
        else
            image = GetComponent<Image>();

        if (!inUI)
            material = spriteRenderer.material;
        else
            material = image.material;
    }

    private IEnumerator FadeInCoroutine(Action action)
    {
        transform.rotation = Quaternion.identity;
        if (!withMaterial)
        {
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.unscaledDeltaTime * transitionSpeed;
                Color color;
                if (!inUI)
                    color = spriteRenderer.color;
                else
                    color = image.color;
                color.a = alpha;
                if (!inUI) 
                spriteRenderer.color = color;
                else
                    image.color = color;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
        else
        {
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.unscaledDeltaTime * transitionSpeed;
                material.SetFloat("_Progress", progress);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
        action();
        yield return new WaitForSecondsRealtime(0.25f);
    }
    private IEnumerator FadeOutCoroutine()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        if (!withMaterial)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.unscaledDeltaTime * 10;
                Color color;
                if (!inUI)
                    color = spriteRenderer.color;
                else
                    color = image.color;
                color.a = alpha;
                spriteRenderer.color = color;
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
        else
        {
            float progress = 1f;
            while (progress > 0f)
            {
                progress -= Time.unscaledDeltaTime * transitionSpeed;
                material.SetFloat("_Progress", progress);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
    }

    private IEnumerator TransitionCoroutine(Action action)
    {
        yield return StartCoroutine(FadeInCoroutine(action));
        StartCoroutine(FadeOutCoroutine());
    }

    public void TransitioN(Action fadeInAction)
    {
        StopAllCoroutines();
        if(sound != string.Empty) G.audio.PlaySound(sound, 1f, transform.position);
        StartCoroutine(TransitionCoroutine(fadeInAction));
    }
}