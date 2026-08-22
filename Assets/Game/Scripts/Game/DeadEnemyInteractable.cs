using UnityEngine;

public class DeadEnemyInteractable : DraggableSmoothDamp
{
    public SpriteRenderer spriteRenderer;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected override void OnRelease()
    {
        if (G.sack != null)
        {
            G.sack.Claim(this);
        }
    }
}
