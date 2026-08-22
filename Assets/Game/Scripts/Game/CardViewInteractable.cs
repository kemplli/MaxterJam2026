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
            G.events.onRelease.Invoke(data.damage);
            G.area.Release(this);
            G.feel.ShakeLight();
            Destroy(gameObject);
        }
    }
}