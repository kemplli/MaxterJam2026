using DG.Tweening;

public class CardViewInteractable : DraggableSmoothDamp
{
    public Cards id;

    private CardData data;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        data = G.s_data.GetCardData(id);
    }

    protected override void OnRelease()
    {
        if (G.selectedEnemy != null)
        {
            G.area.Release(this);

            if (moveable != null) moveable.enabled = false;

            this.enabled = false;

            G.events.onRelease.Invoke(data.damage);

            transform.DOKill();
            transform.DOScale(0, 0.1f)
                .SetUpdate(true) 
                .OnComplete(() => { Destroy(gameObject); G.feel.ShakeLight(); } );
        }
    }

    protected override void PointerEnter()
    {
        G.tooltip.Show(data.name, data.description);
    }

    protected override void PointerExit()
    {
        G.tooltip.Hide();
    }
}