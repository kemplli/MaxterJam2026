using UnityEngine;

public class CardData : MonoBehaviour
{
    public Cards id;

    public int damage;
    public Sprite view;

    public string name;
    [TextArea(5,5)]
    public string description;
}

public enum Cards
{
    Scanner,
    EnteringTheCode,
    Heat,
    Sedative
}