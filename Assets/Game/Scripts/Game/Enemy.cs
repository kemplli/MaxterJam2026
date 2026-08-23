using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;

public class Enemy : ManagedBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 startScale;

    public Enemies id;
    private EnemyData data;

    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private TMP_Text shieldText;

    [SerializeField]
    private GameObject deadPrefab;

    private SpriteRenderer spriteRenderer;

    [NonSerialized]
    public int health;

    [NonSerialized]
    public int shield;

    public EnemyState state;

    protected override void ManagedInitialize()
    {
        startScale = transform.localScale;

        data = G.s_data.GetEnemyData(id);
        health = data.health;
        shield = data.shield;

        state = data.startState;

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
        shieldText.text = shield.ToString();
    }

    private void OnDestroy()
    {
        G.events.onRelease -= TakeDamage;
    }

    public void TakeDamage(int damage)
    {
        if (G.selectedEnemy == this)
        {
            if (shield > 0)
            {
                if (damage == shield)
                {
                    BreakShield();
                    return;
                }
                if (damage > shield)
                {
                    var healthDamage = damage - shield;
                    health -= healthDamage;
                    BreakShield();
                    if (health <= 0)
                    {
                        Death();
                    }
                    return;
                }
                shield -= damage;

                return;
            }
            if (health > 0)
            {
                health -= damage;
            }
        }

        if (health <= 0)
        {
            Death();
        }
    }

    public void BreakShield()
    {
        if (shield > 0)
        {
            shield = 0;
            shieldText.transform.parent.DOScale(0, 0.5f).SetEase(Ease.InOutQuad);
            G.ui.SpawnFloatingMessage("Защита снята!", new Color(1f, 0.647f, 0f));
        }
    }

    public virtual IEnumerator Attack()
    {
        yield return transform.DOPunchPosition(new Vector3(-0.5f, 0f, 0f), 0.2f, vibrato: 1);

     //   if (state != EnemyState.Rotten)
      //  {
      //      G.player.TakeDamage(data.attackDamage);
     //   }
    //    else
     //   {
    //    }
        G.feel.ShakeLight();
        G.ui.SpawnFloatingMessage("Аттака -" + data.attackDamage, Color.red);
        G.gVolCon.VignetteFlash(0f,0.26f,Color.red);

        yield return new WaitForSeconds(0.8f);

        yield break;
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

public enum EnemyState
{
    Normal,
    Frozen,
    Fragile,
    Rotten
}