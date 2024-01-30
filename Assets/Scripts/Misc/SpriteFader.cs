using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFader : MonoBehaviour
{
    public float waitBeforeFadeIn = 0f;
    public float waitBeforeFadeOut = 0f;
    public float fadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;

    private Color initialColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer component is missing!");
        }
    }

    private void Start()
    {
        initialColor = spriteRenderer.color;
    }


    public void FadeIn(float fadeDurationOverride)
    {
        fadeDuration = fadeDurationOverride;
        FadeIn();
    }

    public void FadeIn()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer reference is missing!");
            return;
        }



        StartCoroutine(FadeCanvasGroup(spriteRenderer, spriteRenderer.color, initialColor.a, fadeDuration, waitBeforeFadeIn));
    }

    public void FadeOut(float fadeDurationOverride)
    {
        fadeDuration = fadeDurationOverride;
        FadeOut();
    }
    
    public void FadeOut()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer reference is missing!");
            return;
        }

        StartCoroutine(FadeCanvasGroup(spriteRenderer, spriteRenderer.color, 0f, fadeDuration, waitBeforeFadeOut));
    }

    private IEnumerator FadeCanvasGroup(SpriteRenderer spriteRenderer, Color startColor, float targetAlpha, float duration, float waitBeforeFade)
    {
        if (waitBeforeFade > 0)
            yield return new WaitForSeconds(waitBeforeFade);

        float elapsedTime = 0f;

        Color targetColor = startColor;

        while (elapsedTime < duration)
        {
            targetColor.a = Mathf.Lerp(startColor.a, targetAlpha, elapsedTime / duration);
            spriteRenderer.color = targetColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
