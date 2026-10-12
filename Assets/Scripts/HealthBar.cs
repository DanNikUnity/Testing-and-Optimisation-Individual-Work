using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Character character;
    public string canvasTag = "WorldSpaceCanvas"; // Тег для World Space Canvas

    private void Start()
    {
        if (character == null)
        {
            character = GetComponentInParent<Character>();
        }

        // Найти Canvas World Space в сцене
        GameObject canvasObject = GameObject.FindWithTag(canvasTag);
        if (canvasObject != null)
        {
            transform.SetParent(canvasObject.transform, false);
        }
        else
        {
            Debug.LogWarning("World Space Canvas не найден! Убедитесь, что у Canvas установлен правильный тег.");
        }
    }

    private void Update()
    {
        if (character != null && healthSlider != null)
        {
            healthSlider.value = (float)character.CurrentHealth / character.MaxHealth;
            transform.position = character.transform.position + character.healthBarOffset; // Удержание над головой с учетом смещения
        }
    }
}
