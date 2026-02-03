using UnityEngine;
using UnityEngine.UI;

public class TeaTemperature : MonoBehaviour
{
    [Header("Ustawienia temperatury")]
    public float maxTemperature = 100f;
    private float currentTemperature;
    public float coolingRate = 2f; // Podstawowe tempo stygnięcia
    private float speedMultiplier = 1f; // Mnożnik poziomu trudności

    [Header("Elementy UI")]
    public Image fillImage;

    void Start()
    {
        currentTemperature = maxTemperature;
    }

    void Update()
    {
        if (currentTemperature > 0)
        {
            // Herbata stygnie szybciej w zależności od poziomu (speedMultiplier)
            currentTemperature -= coolingRate * speedMultiplier * Time.deltaTime;
            UpdateUI();
        }
        else
        {
            currentTemperature = 0;
            // GameManager w swojej pętli Update wykryje, że jest 0 i skończy grę.
        }
    }

    void UpdateUI()
    {
        float ratio = currentTemperature / maxTemperature;
        if (fillImage != null) fillImage.fillAmount = ratio;
    }

    // Pozwala GameManagerowi sprawdzić, czy jeszcze gramy
    public float GetCurrentTemperature()
    {
        return currentTemperature;
    }

    // Wywoływane przez GameManager przy każdej nowej planszy
    public void SetSpeedMultiplier(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
        Debug.Log("Herbata stygnie teraz z prędkością: x" + speedMultiplier);
    }

    // Dodaje temperaturę po znalezieniu pary
    public void ReheatTea(float amount)
    {
        currentTemperature = Mathf.Clamp(currentTemperature + amount, 0, maxTemperature);
    }
}