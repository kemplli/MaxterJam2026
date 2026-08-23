using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    [SerializeField] private float maxShift = 2f;      
    [SerializeField] private float smoothSpeed = 3f;   

    private Vector3 _originalPosition;

    void Start()
    {
        _originalPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        float mouseXNormalized = mouseScreenPos.x / Screen.width;
        float mouseYNormalized = mouseScreenPos.y / Screen.height;

        float targetXOffset = (mouseXNormalized - 0.5f) * 2f;
        float targetYOffset = (mouseYNormalized - 0.5f) * 2f;

        Vector3 targetPosition = new Vector3(
            _originalPosition.x + (targetXOffset * maxShift),
            _originalPosition.y + (targetYOffset * maxShift),
            _originalPosition.z 
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
