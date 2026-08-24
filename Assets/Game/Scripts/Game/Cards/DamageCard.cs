using System;
using UnityEngine;

[Serializable]
public class DamageCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.events.onRelease.Invoke(data.damage);
        G.vfx.PlayOneTimeVFX("Scanner", position,Color.white);
    }
}
