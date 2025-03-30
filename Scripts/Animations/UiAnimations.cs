using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Animations
{
    public static class UiAnimations
    {
        public static event Action<float> OnAnimationFinished;
        
        public static IEnumerator AnimateFadeOut(Image image, float duration = 1.0f)
        {
            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Color currentColor = image.color;
                currentColor.a = 1.0f + (-1.0f * (elapsed / duration));
                image.color = currentColor;
                yield return null;
            }
            OnAnimationFinished?.Invoke(elapsed);
            Color endColor = image.color;
            endColor.a = 0.0f;
            image.color = endColor;
        }
    }
}