using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sack : ManagedBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int bodiesCount;

    public void OnPointerEnter(PointerEventData eventData)
    {
        G.sack = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        G.sack = null;
    }

    public void Claim(DeadEnemyInteractable dead)
    {
        Destroy(dead.gameObject);
        bodiesCount++;
        transform.DOScale(transform.localScale + new Vector3(0.2f, 0.2f,0f), 0.5f);
    }
}
