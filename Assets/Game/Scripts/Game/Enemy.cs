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

    private SineWaveRotation waveRotation;

    protected override void ManagedInitialize()
    {
        startScale = transform.localScale;

        data = G.s_data.GetEnemyData(id);
        health = data.health;
        shield = data.shield;

        state = data.startState;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.view;

        waveRotation = GetComponent<SineWaveRotation>();

        G.events.onRelease += TakeDamage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        G.selectedEnemy = this;

        if (!G.main.enemyTurn)
        {
            transform.DOKill();
            transform.DOScale(startScale * 1.2f, 0.25f);
        }
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

        if(shield > 0)
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
                shieldText.transform.parent.DOPunchScale(new Vector3(1f, 1f), 0.25f, vibrato: 1);
             //   transform.DOKill();
             //  transform.DOPunchScale(new Vector3(1f,1f,0f),0.25f,vibrato: 1);

                return;
            }
            if (health > 0)
            {
                health -= damage;
            //    transform.DOKill();
            //    transform.DOPunchScale(new Vector3(1f, 1f, 0f), 0.25f, vibrato: 1);
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
            G.audio.PlaySound("shieldBreak", 0.5f, transform.position);
            G.ui.SpawnFloatingMessage("Защита снята!", new Color(1f, 0.647f, 0f));
        }
    }

    public virtual IEnumerator Attack()
    {
        if (state == EnemyState.Rotten && G.player.data.poison > 0)
        {
            yield break;
        }

        waveRotation.enabled = false;
        transform.DORotate(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        transform.DOPunchPosition(new Vector3(-0.8f, 0f, 0f), 0.2f, vibrato: 1);
        transform.DOPunchRotation(new Vector3(0f,0f,30f),0.2f,vibrato: 1);

        yield return new WaitForSeconds(0.1f);

        if (state != EnemyState.Rotten)
        {
            G.player.TakeDamage(data.attackDamage);
        }
        else
        {
            if (G.player.data.poison <= 0)
            {
                G.player.TakePoison(data.poison);
                G.ui.SpawnFloatingMessage("+" + data.poison + " К Яду", G.darkGreen);
                G.audio.PlaySound("acid", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f));
                G.gVolCon.VignetteFlash(0f,0.26f,G.darkGreen);
                G.feel.ShakeLight();
                G.audio.PlaySound("hitHurt", 1f, transform.position, pitch: new AudioParams.Pitch(0.8f, 1.2f));
                G.player.TakeDamage(data.attackDamage);
            }
            waveRotation.enabled = true;

            yield return new WaitForSeconds(0.8f);

            yield break;
        }
        G.feel.ShakeLight();
        G.ui.SpawnFloatingMessage("Атака - " + data.attackDamage, Color.red);
        G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);

        G.audio.PlaySound("hitHurt", 1f, transform.position, pitch: new AudioParams.Pitch(0.8f, 1.2f));

        waveRotation.enabled = true;

        yield return new WaitForSeconds(0.8f);

        yield break;
    }

    private void Death()
    {
        transform.DOKill();
        if (data.deadPrefab == null)
        {
            var inst = GameObject.Instantiate(deadPrefab, transform.position, deadPrefab.transform.rotation);
            inst.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
            inst.transform.localScale = transform.localScale;
            var dead = inst.GetComponent<DeadEnemyInteractable>();
            dead.name = data.name;
            dead.enemyType = id;
            G.levelManager.deadBodies.Add(dead);
        }
        else
        {
            var inst = GameObject.Instantiate(data.deadPrefab, transform.position, deadPrefab.transform.rotation);
            var dead = inst.GetComponent<DeadEnemyInteractable>();
            dead.name = data.name;
            dead.enemyType = id;
            G.levelManager.deadBodies.Add(dead);
        }
        G.levelManager.currentEnemyCount--;
        G.audio.PlaySound("enemyDeath", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f));

        Destroy(gameObject);
    }
}

public enum Enemies
{
    None,
    Eggs,
    Tomato,
    Milk,
    Cola,
    Sosiski,
    Pelmen,
    PachkaPelmeni,
    Sponge,
    Pringles,
    Glove,
    Mentos,
    SmallSosiska,
    EggsTutorial
}

public enum EnemyState
{
    Normal,
    Frozen,
    Fragile,
    Rotten
}