using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardViewInteractable : DraggableSmoothDamp
{
    public Cards id;

    private CardData data;

    [SerializeReference, SubclassSelector]
    private ICard logic;

    private int currentCooldownAmount;

    private SpriteRenderer spriteRenderer;

    private float yMax = -0.17f;
    private float yMin = -0.827f;

    private Transform full;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        data = G.s_data.GetCardData(id);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = data.view;


        if (data.cooldownAmount > 0)
        {
            full = transform.GetChild(0);
            Debug.Log("DD" + id.ToString());
            G.events.onTurnEnd += UpdateCooldown;
            currentCooldownAmount = data.cooldownAmount;
        }
    }

    private void UpdateCooldown()
    {
        if (currentCooldownAmount > 0)
        {
            currentCooldownAmount--;
        }
    }

    public override void ManagedUpdate()
    {
        if (data.cooldownAmount > 0)
        {
            float yTarget = Mathf.Lerp(yMin, yMax, (float)currentCooldownAmount / (float)data.cooldownAmount);
            full.transform.localPosition = Vector2.Lerp(full.transform.localPosition,new Vector2(0,yTarget),Time.deltaTime * 10);
        }
    }

    protected override void OnRelease()
    {
        if (G.main.enemyTurn) return;

        if (G.selectedEnemy != null && data.whenRelease)
        {
            if (!data.cooldown)
            {
                G.area.Release(this);

                if (moveable != null) moveable.enabled = false;

                this.enabled = false;

                logic.Execute(this, data, G.selectedEnemy,transform.position);

                transform.DOKill();
                transform.DOScale(0, 0.1f)
                    .SetUpdate(true)
                    .OnComplete(() => { Destroy(gameObject); G.feel.ShakeLight(); });
            }
            else
            {
                if (data.cooldownAmount == 0 || (data.cooldownAmount != 0 && currentCooldownAmount == 0))
                {
                    logic.Execute(this, data, G.selectedEnemy,transform.position);
                    G.feel.ShakeLight();
                    currentCooldownAmount = data.cooldownAmount;
                }
                else
                {
                    G.feel.ShakeLight();
                    G.ui.SpawnFloatingMessage("Подожди перезарядку!", Color.red);
                    G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);
                }
            }
        }
    }

    protected override void PointerEnter()
    {
        G.tooltip.Show(data.name, data.description + " Осталось ходов до использования: " + currentCooldownAmount);
    }

    protected override void PointerExit()
    {
        G.tooltip.Hide();
    }

    protected override void PointerDown()
    {
        if (G.main.enemyTurn) return;
        if (!data.whenRelease)
        {
            if (!data.cooldown)
            {
                G.area.Release(this);

                if (moveable != null) moveable.enabled = false;

                this.enabled = false;

                logic.Execute(this, data, G.selectedEnemy, transform.position);

                transform.DOKill();
                transform.DOScale(0, 0.1f)
                    .SetUpdate(true)
                    .OnComplete(() => { Destroy(gameObject); G.feel.ShakeLight(); });
            }
            else
            {
                if (data.cooldownAmount == 0 || (data.cooldownAmount != 0 && currentCooldownAmount == 0))
                {
                    logic.Execute(this, data, G.selectedEnemy,transform.position);
                    G.feel.ShakeLight();
                    currentCooldownAmount = data.cooldownAmount;
                }
                else
                {
                    G.feel.ShakeLight();
                    G.ui.SpawnFloatingMessage("Подожди перезарядку!",Color.red);
                    G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);
                    currentCooldownAmount = data.cooldownAmount;
                }
            }
        }
    }

    private void OnDisable()
    {
        G.events.onTurnEnd -= UpdateCooldown;
    }
}

public interface ICard
{
    public void Execute(CardViewInteractable card,CardData data,Enemy enemy,Vector2 position);
}
