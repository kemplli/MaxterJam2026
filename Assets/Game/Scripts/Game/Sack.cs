using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Sack : DraggableSmoothDamp
{
    public int bodiesCount;

    private int maxBodiesCount = 3;

    private List<Enemies> typesIn = new List<Enemies>();

    bool exited;

    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private SpriteRenderer renderer;

    private string description = "Внутри:";

    public bool full => bodiesCount >= maxBodiesCount;

    private Synergy synergy;

    public SynergyTypes synergyTypes = SynergyTypes.None;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        renderer = GetComponent<SpriteRenderer>();

        renderer.sprite = sprites[0];

        synergy = new Synergy();

        synergy.sack = this;
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
        G.audio.PlaySound("bag", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f), repetition: new AudioParams.Repetition(0.05f));
        G.levelManager.deadBodies.Remove(dead);
        typesIn.Add(dead.enemyType);
        Destroy(dead.gameObject);
        bodiesCount++;
        transform.DOScale(transform.localScale + new Vector3(0.2f, 0.2f,0f), 0.5f);
        if (synergy.first != Enemies.None && synergy.second != Enemies.None && synergy.last == Enemies.None)
        {
            synergy.last = dead.enemyType;
            description += "," + dead.name;
        }
        if (synergy.first != Enemies.None && synergy.second == Enemies.None)
        {
            synergy.second = dead.enemyType;
            description += "," + dead.name;
        }
        if (synergy.first == Enemies.None)
        {
            synergy.first = dead.enemyType;
            description += " " + dead.name;
        }

        if (synergy.first != Enemies.None && synergy.second != Enemies.None) G.synergy.synergies.Add(synergy);

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
        if (bodies == 0) renderer.sprite = sprites[0];
        if(bodies == 1) renderer.sprite = sprites[1];
        if(bodies == 3) renderer.sprite = sprites[2];
    }

    protected override void PointerUp()
    {
       // G.audio.PlaySound("bag", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f), repetition: new AudioParams.Repetition(0.05f));

        //if (defaultSortingLayer == "Default")
        //{
        //    if (bodiesCount > 0)
        //    {
        //        G.tooltip.Show("Мешок", description);
        //    }
        //    else
        //    {
        //        G.tooltip.Show("Мешок", "Пусто");
        //    }
        //}
    }

    protected override void PointerDown()
    {
        G.audio.PlaySound("bag", 1f, transform.position, pitch: new AudioParams.Pitch(0.9f, 1.1f),repetition: new AudioParams.Repetition(0.05f));
    }
}

[Serializable]
public class Synergy
{
    public Enemies first;
    public Enemies second;
    public Enemies last;

    public SynergyTypes type;

    public Sack sack;
}

public enum SynergyTypes
{
    None,
    ColaMentos,
    MechanicPringles
}