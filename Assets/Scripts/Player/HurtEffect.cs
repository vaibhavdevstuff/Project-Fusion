using UnityEngine;
using UnityEngine.Rendering;

public class HurtEffect : MonoBehaviour
{
    public float transitionSpeed = 0.5f; 
    private Volume hurtEffectVolume;

    private void Start()
    {
        hurtEffectVolume = GetComponent<Volume>();
    }

    private void SmoothlyTransitionToZero()
    {
        hurtEffectVolume.weight = Mathf.Lerp(hurtEffectVolume.weight, 0f, Time.deltaTime * transitionSpeed);
    }

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
