using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controls a visual effect for indicating player damage.
/// </summary>
public class HurtEffect : MonoBehaviour
{
    public float transitionSpeed = 0.5f; 
    private Volume hurtEffectVolume;

    private void Start()
    {
        hurtEffectVolume = GetComponent<Volume>();
    }

    /// <summary>
    /// Smoothly transitions the hurt effect to zero weight.
    /// </summary>
    private void SmoothlyTransitionToZero()
    {
        hurtEffectVolume.weight = Mathf.Lerp(hurtEffectVolume.weight, 0f, Time.deltaTime * transitionSpeed);
    }

    /// <summary>
    /// Triggers the hurt effect by increasing its weight.
    /// </summary>
    public void GotHurt()
    {
        hurtEffectVolume.weight = Mathf.Clamp01(hurtEffectVolume.weight + 0.25f);
    }

    private void Update()
    {
        if (hurtEffectVolume.weight > 0)
        {
            SmoothlyTransitionToZero();
        }
    }
}
