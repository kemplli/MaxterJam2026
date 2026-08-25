using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingMessage : ManagedBehaviour
{
    [SerializeField]
    private TMP_Text text;

    public float timeIn;

    public bool withShake;

    [SerializeField]
    private TMP_LinkShaker shaker;

    protected override void ManagedInitialize()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        if (withShake) shaker.speed = 2;

        text.DOFade(1, 0.5f);
        transform.DOScale(1, 0.2f);
        transform.DOPunchRotation(new Vector3(0, 0, -50f), 0.2f);
        transform.DOMoveY(transform.position.y + 5, 1.5f);
        yield return new WaitForSeconds(0.5f);
        text.DOFade(0, 0.4f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public override void ManagedUpdate()
    {
        if (withShake)
        {
            shaker.speed = 4;
            shaker.magnitude = 80;
        }
    }
}
