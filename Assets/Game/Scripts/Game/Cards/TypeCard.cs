using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public class TypeCard : ICard
{
    public void Execute(CardViewInteractable card, CardData data, Enemy enemy, Vector2 position)
    {
        G.events.onRelease.Invoke(data.damage);
        G.vfx.PlayOneTimeVFX("Type", position, Color.white);
        //G.audio.PlaySound("beep", 1f, position, pitch: new AudioParams.Pitch(0.9f, 1.1f));
        G.audio.PlaySound("typingClick",1f,position);
        G.audio.PlaySound("hitCard", 1f, position);
    }
}