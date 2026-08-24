using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Vector3 startSize;

    private void Awake()
    {
        startSize = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = startSize;
    }
}