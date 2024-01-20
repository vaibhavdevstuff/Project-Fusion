using UnityEngine;

public static class CustomExtensions
{
    public static void SetRenderLayerInChildren(this Transform transform, int layer)
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            childTransform.gameObject.layer = layer;
        }
    }

    #region Animator GetAnimationTime

    public static float GetCurrentPlayingAnimationTime(this Animator animator, int layerIndex, bool DebugClip = false)
    {
        return GetCurrentPlayingAnimationTime(animator, layerIndex, 1, DebugClip);
    }
    public static float GetCurrentPlayingAnimationTime(this Animator animator, int layerIndex, float AnimationMultiplier, bool DebugClip = false)
    {
        if (AnimationMultiplier <= 0) AnimationMultiplier = 1;

        AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);

        float clipLength = 0;

        if (m_CurrentClipInfo.Length > 0)
        {
            clipLength = m_CurrentClipInfo[0].clip.length;
        }

        float FinalClipTime = clipLength / AnimationMultiplier;

#if UNITY_EDITOR
        //Debug
        if (DebugClip)
            Debug.Log($"Current Clip: {m_CurrentClipInfo[0].clip.name} \n Actual Clip Time: {clipLength} \n Final Clip Time: {FinalClipTime}");
#endif


        return float.IsInfinity(FinalClipTime) ? 1f : FinalClipTime;


    }

    #endregion












}
