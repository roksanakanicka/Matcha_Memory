using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeaTemperature : MonoBehaviour
{
    [Header("Ustawienia temperatury")]
    public float maxTemperature = 100f;
    private float currentTemperature;
    public float coolingRate = 2f; // Ile stopni na sekundę traci herbata

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
            // Logika stygnięcia: T_nowa = T_stara - (tempo * czas)
            currentTemperature -= coolingRate * Time.deltaTime;
            UpdateUI();
        }
        else
        {
            Debug.Log("Herbata wystygła! Koniec gry.");
            currentTemperature = 0;
            // Tutaj możesz wywołać zdarzenie końca gry
        }
    }

    void UpdateUI()
    {
        // Obliczamy stosunek temperatury (wartość od 0 do 1)
        float ratio = currentTemperature / maxTemperature;

        // Aktualizujemy wysokość paska
        fillImage.fillAmount = ratio;
    }

    // Tę funkcję możesz wywołać ze skryptu Roksany, gdy gracz znajdzie parę!
    public void ReheatTea(float amount)
    {
        currentTemperature = Mathf.Clamp(currentTemperature + amount, 0, maxTemperature);
    }
}