using System;

[Serializable]
public class DamageCard : ICard
{
    public void Execute(CardViewInteractable card,CardData data,Enemy enemy)
    {
        G.events.onRelease.Invoke(data.damage);
    }
}
