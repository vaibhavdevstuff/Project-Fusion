using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float waitBeforeFadeIn = 0f;
    public float waitBeforeFadeOut = 0f;
    public float fadeDuration = 1.0f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is missing!");
        }
    }

    private void OnEnable()
    {
        FadeIn();
    }

    public void FadeIn(float fadeDurationOverride)
    {
        fadeDuration = fadeDurationOverride;
        FadeIn();
    }

    public void FadeIn()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup reference is missing!");
            return;
        }

        canvasGroup.alpha = 0f;

        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeDuration, waitBeforeFadeIn, false));
    }

    public void FadeOut(float fadeDurationOverride)
    {
        fadeDuration = fadeDurationOverride;
        FadeOut();
    }

    public void FadeOut()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup reference is missing!");
            return;
        }

        canvasGroup.alpha = 1f;

        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration, waitBeforeFadeOut, true));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration, float waitBeforeFade , bool deactivate = false)
    {
        if(waitBeforeFade > 0)
            yield return new WaitForSeconds(waitBeforeFade);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if(deactivate)
            this.gameObject.SetActive(false);
    }
}
