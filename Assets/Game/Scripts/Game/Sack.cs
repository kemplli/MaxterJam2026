using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Sack : DraggableSmoothDamp
{
    public int bodiesCount;

    private int maxBodiesCount = 2;

    private List<Enemies> typesIn = new List<Enemies>();

    bool exited;

    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private SpriteRenderer renderer;

    private string description = "Внутри:";

    public bool full => bodiesCount >= maxBodiesCount;

    private Synergy synergy;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        renderer = GetComponent<SpriteRenderer>();

        renderer.sprite = sprites[0];

        synergy = new Synergy();
    }

    protected override void PointerEnter()
    {
        G.sack = this;

        if (defaultSortingLayer == "Default")
        {
            if (bodiesCount > 0)
            {
                G.tooltip.Show("Мешок", description);
            }
            else
            {
                G.tooltip.Show("Мешок", "Пусто");
            }
        }
    }

    protected override void PointerExit()
    {
        G.sack = null;

        G.tooltip.Hide();
    }

    public void Claim(DeadEnemyInteractable dead)
    {
        G.levelManager.deadBodies.Remove(dead);
        typesIn.Add(dead.enemyType);
        Destroy(dead.gameObject);
        bodiesCount++;
        transform.DOScale(transform.localScale + new Vector3(0.2f, 0.2f,0f), 0.5f);
        description += " " + dead.name;
        if (synergy.first != Enemies.None && synergy.last == Enemies.None)
        {
            synergy.last = dead.enemyType;
            Debug.Log("Second");
        }
        if (synergy.first == Enemies.None)
        {
            synergy.first = dead.enemyType;
            Debug.Log(synergy);
        }

        if (synergy.first != Enemies.None && synergy.last != Enemies.None) G.synergy.synergies.Add(synergy);

        UpdateSprite(bodiesCount);
    }

    public override void ManagedUpdate()
    {
        if (isDragging)
        {
            G.tooltip.Hide();
        }
    }

    private void UpdateSprite(int bodies)
    {
        renderer.sprite = sprites[bodies];
    }

    protected override void PointerUp()
    {

        if (defaultSortingLayer == "Default")
        {
            if (bodiesCount > 0)
            {
                G.tooltip.Show("Мешок", description);
            }
            else
            {
                G.tooltip.Show("Мешок", "Пусто");
            }
        }
    }

}

[Serializable]
public class Synergy
{
    public Enemies first;
    public Enemies last;

    public SynergyTypes type;
}

public enum SynergyTypes
{
    None,
    ColaMentos
}