using UnityEngine;
using UnityEngine.Rendering;

public class FullWithSacks : ManagedBehaviour
{
    protected override void ManagedOnTriggerExit(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Sack>(out Sack sack))
        {
            sack.transform.parent = null;
            sack.transform.localScale = Vector3.one;
            sack.defaultSortingLayer = "Default";
        }
    }
}