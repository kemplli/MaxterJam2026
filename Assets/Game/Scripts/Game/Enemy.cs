using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class Enemy : ManagedBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 startScale;

    public int health;

    [SerializeField]
    private TMP_Text healthText;

    protected override void ManagedInitialize()
    {
        startScale = transform.localScale;

        G.events.onRelease += TakeDamage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        G.selectedEnemy = this;

        transform.DOKill();
        transform.DOScale(startScale * 1.2f,0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        G.selectedEnemy = null;

        transform.DOKill();
        transform.DOScale(startScale, 0.25f);
    }

    public override void ManagedUpdate()
    {
        healthText.text = health.ToString();
    }

    private void OnDestroy()
    {
        G.events.onRelease -= TakeDamage;
    }

    public void TakeDamage(int damage)
    {
        if (G.selectedEnemy == this)
        {
            if (health > 0)
                health -= damage;
        }

        if (health <= 0)
        {
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
