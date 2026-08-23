using UnityEngine;
using TMPro;
using DG.Tweening;

public class Tooltip : ManagedBehaviour
{
    [SerializeField]
    private TMP_Text name;

    [SerializeField]
    private TMP_Text description;

    [SerializeField]
    private Vector2 offset;

    protected override void ManagedInitialize()
    {
        G.tooltip = this;
    }

    public void Show(string name,string description)
    {
        this.name.text = name;
        this.description.text = description;
        this.name.transform.parent.transform.DOKill();
        this.name.transform.parent.transform.DOScale(new Vector3(0.8f,0.8f,1f), 0.25f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        this.name.transform.parent.transform.DOKill();
        this.name.transform.parent.transform.DOScale(0, 0.15f); 
    }

    public override void ManagedUpdate()
    {
        transform.position = GameMath.MousePos2D() + offset;
    }
}