using UnityEngine;
using System.Collections.Generic;

public class CardManager : ManagedBehaviour
{
    [SerializeField]
    private List<CardViewInteractable> cooldownCards = new List<CardViewInteractable>();

    [SerializeField]
    private List<CardViewInteractable> basicCards = new List<CardViewInteractable>();

    protected override void ManagedInitialize()
    {
        GiveCooldownCards();
        GiveBasicCards();

       // G.events.onTurnEnd += GiveBasicCards;
    }

    private void GiveCooldownCards()
    {
        foreach (var card in cooldownCards)
        {
            var inst = Instantiate(card,Vector2.zero,Quaternion.identity);
            G.area.Claim(inst.GetComponent<CardViewInteractable>());
        }
    }

    private void GiveBasicCards()
    {
        foreach (var card in basicCards)
        {
            var inst = Instantiate(card, Vector2.zero, Quaternion.identity);
            G.area.Claim(inst.GetComponent<CardViewInteractable>());
        }
    }

    private void OnDestroy()
    {
        G.events.onTurnEnd -= GiveBasicCards;
    }
}