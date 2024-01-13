using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class CustomExtensions
    {
        public static void SetRenderLayerInChildren(this Transform transform, int layer)
        {
            foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
            {
                childTransform.gameObject.layer = layer;
            }
        }














    }





}
