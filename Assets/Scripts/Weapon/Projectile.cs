using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Setup")]
    public float Speed = 80f;
    public float MaxDistance = 100f;
    public float LifeTimeAfterHit = 2f;

    [Header("Impact Setup")]
    public GameObject ProjectileObject;
    public GameObject HitEffectPrefab;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float startTime;
    private float duration;

    /// <summary>
    /// Set where the projectile visual should land.
    /// </summary>
    public void SetHit(Vector3 hitPosition)
    {
        targetPosition = hitPosition;
    }

    private void Start()
    {
        startPosition = transform.position;

        if (targetPosition == Vector3.zero)
        {
            targetPosition = startPosition + transform.forward * MaxDistance;
        }

        duration = Vector3.Distance(startPosition, targetPosition) / Speed;
        startTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        float time = Time.timeSinceLevelLoad - startTime;

        if (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
        }
        else
        {
            transform.position = targetPosition;
            FinishProjectile();
        }
    }

    private void FinishProjectile()
    {
        Destroy(gameObject);
    }




}

