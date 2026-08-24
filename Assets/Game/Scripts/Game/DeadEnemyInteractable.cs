using UnityEngine;

public class DeadEnemyInteractable : DraggableSmoothDamp
{
    public SpriteRenderer spriteRenderer;

    public Enemies enemyType;

    public string name;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected override void OnRelease()
    {
        if (G.sack != null)
        {
            if (!G.sack.full)
            {
                G.sack.Claim(this);
            }
            else
            {
                G.feel.ShakeLight();
                G.ui.SpawnFloatingMessage("Мешок полон!", Color.red);
                G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);
            }
        }
    }
}
