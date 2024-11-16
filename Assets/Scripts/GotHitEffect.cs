using UnityEngine;
using UnityEngine.UI;

public class GotHitEffect : MonoBehaviour
{
    [Header("Hit Effect Settings")]
    public Image hitEffectImage; // Assign your full-screen UI image here
    public Color hitColor = new Color(1, 0, 0, 0.5f); // Default semi-transparent red
    public float fadeDuration = 0.5f; // Time for the effect to fade out

    private bool isFading = false;
    private float fadeTimer = 0f;

    private void Update()
    {
        if (isFading)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0f)
            {
                fadeTimer = 0f;
                isFading = false;
            }

            // Gradually reduce alpha
            float alpha = fadeTimer / fadeDuration;
            Color currentColor = hitColor;
            currentColor.a = Mathf.Lerp(0, hitColor.a, alpha);
            hitEffectImage.color = currentColor;
        }
    }

    public void TriggerHitEffect()
    {
        if (hitEffectImage == null)
        {
            Debug.LogError("Hit Effect Image is not assigned!");
            return;
        }

        isFading = true;
        fadeTimer = fadeDuration;
        hitEffectImage.color = hitColor; // Set the initial hit color
    }
}
