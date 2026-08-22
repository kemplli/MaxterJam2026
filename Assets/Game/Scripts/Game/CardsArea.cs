using UnityEngine;
using System.Collections.Generic;

public class CardsArea : ManagedBehaviour
{
    public List<CardViewInteractable> cards = new List<CardViewInteractable>();

    [SerializeField]
    private float maxWidth;

    [SerializeField]
    private float maxSpacing;

    [SerializeField]
    private float posY;

    protected override void ManagedInitialize()
    {
        G.area = this;
    }

    public override void ManagedUpdate()
    {
        UpdateCardsPosition();
    }

    private void UpdateCardsPosition(bool immediate = false)
    {
        float spacing = maxWidth / cards.Count;
        spacing = Mathf.Min(spacing, maxSpacing);

        float leftAnchor = (cards.Count * -spacing * 0.5f) + (spacing * 0.5f);
        foreach (var card in cards)
        {
            if (!card.isDragging)
            {
                int index = cards.IndexOf(card);
                float xPos = leftAnchor + (spacing * index);
                card.transform.position = Vector2.Lerp(card.transform.position, new Vector2(xPos, posY),
                    immediate ? 1f : Time.deltaTime * 10f);
            }
        }
    }

    public void Claim(CardViewInteractable card)
    {
        cards.Add(card);
    }

    public void Release(CardViewInteractable card)
    {
        cards.Remove(card);
    }
}
