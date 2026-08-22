using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System;

public class Enemy : ManagedBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 startScale;

    public Enemies id;
    private EnemyData data;

    [SerializeField]
    private TMP_Text healthText;

    [SerializeField]
    private GameObject deadPrefab;

    private SpriteRenderer spriteRenderer;

    [NonSerialized]
    public int health;


    protected override void ManagedInitialize()
    {
        startScale = transform.localScale;

        data = G.s_data.GetEnemyData(id);
        health = data.health;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.view;

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
            Death();
        }
    }

    private void Death()
    {
        transform.DOKill();
        var inst = GameObject.Instantiate(deadPrefab, transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-180f, 180f)));
        G.levelManager.currentEnemyCount--;
        Destroy(gameObject);
    }
}

public enum Enemies
{
    Basic,
    Eggs
}
