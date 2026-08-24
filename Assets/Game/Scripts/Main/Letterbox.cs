using UnityEngine;

[ExecuteInEditMode]
public class Letterbox : MonoBehaviour
{
    public float targetAspectX = 16f;
    public float targetAspectY = 9f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        ApplyLetterbox();
    }

    void Update()
    {
        ApplyLetterbox();
    }

    void ApplyLetterbox()
    {
        if (cam == null) return;

        float targetAspect = targetAspectX / targetAspectY;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}