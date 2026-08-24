using System;
using UnityEngine;

[Serializable]
public class SuperCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.events.onRelease.Invoke(9999);
        G.player.TakeDamage(data.vampireDamage);
    }
}
