using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class Lever : ManagedBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private bool holding;

    [SerializeField] 
    private float rotationSpeed = 45f;

    private Vector3 startRotation;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<Vector2> currentDestinations = new List<Vector2>();

    private float _enteringSpeed;

    [SerializeField]
    private float enteringSpeed;

    [SerializeField]
    private float cameraShakingMagnitude;


    private float currentMovingProgress = 0f;

    protected override void ManagedInitialize()
    {
        G.lever = this;

        startRotation = transform.rotation.eulerAngles;
    }

    public override void ManagedUpdate()
    {
        if (!holding)
        {
            G.cameraShaker.shakeAmount = Mathf.Lerp(G.cameraShaker.shakeAmount, 0, Time.deltaTime * 5);
            return;
        }

        transform.DOKill();

        var dir = GameMath.DirectionToMouse2D(transform.position);
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (targetAngle > 180f) targetAngle -= 360f;
        if (targetAngle < -180f) targetAngle += 360f;
        float clampedTargetAngle = Mathf.Clamp(targetAngle, -16f, 0f);

        float currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(currentAngle, clampedTargetAngle, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        if (currentMovingProgress < 0.95f)
        {
            MoveEnemies(clampedTargetAngle);
        }
        else
        {
            G.cameraShaker.shakeAmount = Mathf.Lerp(G.cameraShaker.shakeAmount, 0, Time.deltaTime * 5);
        }
    }

    private void MoveEnemies(float clampedTargetAngle)
    {
        _enteringSpeed = Mathf.InverseLerp(0, -16f, clampedTargetAngle) * enteringSpeed;

        var t = _enteringSpeed / enteringSpeed;
        G.cameraShaker.shakeAmount = Mathf.Lerp(0, cameraShakingMagnitude, _enteringSpeed / enteringSpeed);

        for (int i = 0; i < currentEnemies.Count; i++)
        {
            var dest = currentDestinations[i];

            var enemy = currentEnemies[i];

            enemy.transform.position = Vector2.Lerp(enemy.transform.position, (Vector3)dest, Time.deltaTime * _enteringSpeed);
            currentMovingProgress = Mathf.InverseLerp(dest.x + 10, dest.x, enemy.transform.position.x);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;

        transform.DORotate(startRotation, 0.25f).SetEase(Ease.OutBack,7f);
    }

    public void FullWithEnemies(List<GameObject> enemies)
    {
        GameMath.FullListWithOther(enemies, ref currentEnemies);
    }

    public void FullWithDestinations(List<Vector2> dest) {
        GameMath.FullListWithOther(dest, ref currentDestinations);
    }
}
