using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class HeatCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data,Enemy enemy,Vector2 position)
    {
        if (enemy.state != EnemyState.Frozen)
        {
            G.events.onRelease.Invoke(data.damage);
        }
        else
        {
            enemy.BreakShield();
            G.gVolCon.VignetteFlash(0f, 0.26f, new Color(1f, 0.647f, 0f));
            G.events.onRelease.Invoke(data.damage * 2);
        }
    }
}
