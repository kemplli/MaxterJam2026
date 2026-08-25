using System;
using UnityEngine;

[Serializable]
public class HealCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.player.Heal(data.damage);
        G.ui.SpawnFloatingMessage("+" + data.damage + " к здоровью!", Color.green);
        G.gVolCon.VignetteFlash(0f, 0.26f, Color.green);
        G.events.onHealing?.Invoke();
        G.audio.PlaySound("Heal", 1f, position);
    }
}