using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingMessage : ManagedBehaviour
{
    [SerializeField]
    private TMP_Text text;

    protected override void ManagedInitialize()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        text.DOFade(1, 0.5f);
        transform.DOScale(1, 0.2f);
        transform.DOMoveY(transform.position.y + 5, 1.5f);
        yield return new WaitForSeconds(0.5f);
        text.DOFade(0, 0.4f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
