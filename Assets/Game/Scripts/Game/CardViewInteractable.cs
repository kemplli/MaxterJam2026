using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardViewInteractable : DraggableSmoothDamp
{
    public Cards id;

    private CardData data;

    [SerializeReference,SubclassSelector]
    private ICard logic;

    protected override void ManagedInitialize()
    {
        base.ManagedInitialize();
        data = G.s_data.GetCardData(id);
    }

    protected override void OnRelease()
    {
        if (G.selectedEnemy != null && data.whenRelease)
        {
            G.area.Release(this);

            if (moveable != null) moveable.enabled = false;

            this.enabled = false;

            logic.Execute(this, data,G.selectedEnemy);

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

    protected override void PointerDown()
    {
        if (!data.whenRelease)
        {
            G.area.Release(this);

            if (moveable != null) moveable.enabled = false;

            this.enabled = false;

            logic.Execute(this,data,G.selectedEnemy);

            transform.DOKill();
            transform.DOScale(0, 0.1f)
                .SetUpdate(true)
                .OnComplete(() => { Destroy(gameObject); G.feel.ShakeLight(); });
        }
    }
}

public interface ICard
{
    public void Execute(CardViewInteractable card,CardData data,Enemy enemy);
}
