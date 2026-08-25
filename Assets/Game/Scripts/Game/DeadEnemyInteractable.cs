using DG.Tweening;
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
                this.enabled = false;
                transform.DOScale(0, 0.25f);
                G.sack.Claim(this);
            }
            else
            {
                G.feel.ShakeLight();
                G.ui.SpawnFloatingMessage("Мешок полон!", Color.red);
                G.audio.PlaySound("hitHurt", 1f, transform.position, pitch: new AudioParams.Pitch(1.2f));
                G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);
            }
        }
    }
}
