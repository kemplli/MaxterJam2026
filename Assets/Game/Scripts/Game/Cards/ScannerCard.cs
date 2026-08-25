using System;
using UnityEngine;

[Serializable]
public class ScannerCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.events.onRelease.Invoke(data.damage);
        G.vfx.PlayOneTimeVFX("Scanner", position, Color.white);
        G.audio.PlaySound("beep", 0.7f, position, pitch: new AudioParams.Pitch(0.9f, 1f));
        G.audio.PlaySound("hitCard", 1f, position);
    }
}
