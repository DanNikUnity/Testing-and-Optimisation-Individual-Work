using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro
using Bubbles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellsText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Reference to the TextMeshPro component
    public float fadeDuration = 2f; // Duration for the text to fade out
    private Coroutine fadeCoroutine; // Store the current fade coroutine

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {

    }

    // Method to change text and start the fade-out process
    public void ChangeTextAndFade(string newText)
    {
        // Stop any ongoing fade-out coroutine
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Change the text
        textMeshPro.text = newText;

        // Reset alpha to 1 before starting the fade-out
        Color currentColor = textMeshPro.color;
        textMeshPro.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);

        // Start the fade-out coroutine
        fadeCoroutine = StartCoroutine(FadeOutText());
    }

    // Coroutine to slowly fade out the text
    private IEnumerator FadeOutText()
    {
        float elapsedTime = 0f;

        // Save the original color of the text
        Color originalColor = textMeshPro.color;

        // Gradually fade out the text over the fadeDuration
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration); // Fade from original to 0 alpha
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // Apply new alpha value

            yield return null; // Wait for the next frame
        }

        // Ensure the text is fully transparent at the end
        textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
