using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class CardsArea : ManagedBehaviour
{
    public List<CardViewInteractable> cards = new List<CardViewInteractable>();

    [SerializeField]
    private float maxWidth;

    [SerializeField]
    private float maxSpacing;

    [SerializeField]
    private float posY;

    [SerializeField]
    private float maxAngle;

    [SerializeField]
    private float yPosIntensity;

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
        if (cards.Count == 0) return;

        float spacing = maxWidth / cards.Count;
        spacing = Mathf.Min(spacing, maxSpacing);
        float leftAnchor = (cards.Count * -spacing * 0.5f) + (spacing * 0.5f);

        float t = immediate ? 1f : Time.deltaTime * 10;

        for (int index = 0; index < cards.Count; index++)
        {
            var card = cards[index];

            if (card.isDragging) continue;

            float xPos = leftAnchor + (spacing * index) - 1f; 

            float normalizeIndex = 0f;
            if (cards.Count > 1)
            {
                normalizeIndex = (float)index / (cards.Count - 1);
                normalizeIndex = (normalizeIndex * 2f) - 1f;      
            }

            float cardAngle = -normalizeIndex * maxAngle;
            Quaternion targetRot = Quaternion.Euler(0, 0, cardAngle);

            float yOffset = (1f - (normalizeIndex * normalizeIndex)) * yPosIntensity;
            float targetY = posY + yOffset;

            Vector2 targetPos = new Vector2(xPos, targetY);

            card.transform.position = Vector2.Lerp(card.transform.position, targetPos, t);
            card.transform.rotation = Quaternion.Lerp(card.transform.rotation, targetRot, t);
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
