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
    private Vector3 hitNormal;

    private bool showHitEffect = false;
    private GameObject hitEffectPrefab;

    private float startTime;
    private float duration;

    /// <summary>
    /// Set where the projectile visual should land.
    /// </summary>
    /// <param name="hitPosition">Position where projectile hit geometry</param>
    /// <param name="hitNormal">Normal of the hit surface</param>
    /// <param name="showHitEffect">Whether projectile impact should be displayed
    /// (e.g. we don't want static impact effect displayed on other player's body)</param>
    public void SetHit(Vector3 hitPosition, Vector3 hitNormal, bool showHitEffect)
    {
        targetPosition = hitPosition;
        this.showHitEffect = showHitEffect;
        this.hitNormal = hitNormal;
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

        Debug.Log($"{startPosition} | {targetPosition}");
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
        if (showHitEffect == false)
        {
            // No hit effect, destroy immediately.
            Destroy(gameObject);
            return;
        }

        // Stop updating projectile visual.
        enabled = false;

        if (ProjectileObject != null)
        {
            ProjectileObject.SetActive(false);
        }

        if (HitEffectPrefab != null)
        {
            Instantiate(HitEffectPrefab, targetPosition, Quaternion.LookRotation(hitNormal), transform);
        }

        Destroy(gameObject, LifeTimeAfterHit);
    }
}

