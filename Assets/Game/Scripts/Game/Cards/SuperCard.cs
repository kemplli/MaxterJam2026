using System;
using UnityEngine;

[Serializable]
public class SuperCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.events.onRelease.Invoke(9999);
        G.player.TakeDamage(data.vampireDamage);
        G.audio.PlaySound("lethal_hit", 1f, position);
        G.ui.SpawnFloatingMessage("9999 - Урон!",Color.red);
        G.gVolCon.VignetteFlash(0f, 0.4f, Color.red);
    }
}
