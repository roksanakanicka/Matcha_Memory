using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{

    public int rows = 4;
    public int columns = 4;


    public Vector2 baseCellSize = new Vector2(100, 150);
    public float cardScale = 1.0f;
    public float spacingPercentage = 0.1f;


    public RectTransform gridContainer;
    public List<GameObject> cardPrefabs; // Tu przeciągnij swoje warianty (Matcha, Sencha itd.)



    public void GenerateGrid()
    {
        // 1. Dynamiczna konfiguracja GridLayoutGroup
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            float newWidth = baseCellSize.x * cardScale;
            float newHeight = baseCellSize.y * cardScale;
            gridLayout.cellSize = new Vector2(newWidth, newHeight);

            gridLayout.spacing = new Vector2(newWidth * spacingPercentage, newHeight * spacingPercentage);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
        }

        // 2. Czyszczenie starej siatki
        foreach (Transform child in gridContainer) { Destroy(child.gameObject); }

        // 3. Obliczanie całkowitej liczby kart
        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Siatka musi mieć parzystą liczbę pól!");
            return;
        }

        // --- NOWA LOGIKA BALANSU ---
        // 4. Tasujemy listę dostępnych prefabów, aby za każdym razem gra wybierała inne herbaty
        List<GameObject> availablePrefabs = new List<GameObject>(cardPrefabs);
        for (int i = 0; i < availablePrefabs.Count; i++)
        {
            GameObject temp = availablePrefabs[i];
            int randomIndex = Random.Range(i, availablePrefabs.Count);
            availablePrefabs[i] = availablePrefabs[randomIndex];
            availablePrefabs[randomIndex] = temp;
        }

        // 5. Tworzymy listę par na planszę
        List<GameObject> cardsToSpawn = new List<GameObject>();
        for (int i = 0; i < totalCards / 2; i++)
        {
            // Używamy modulo (%), aby brać herbaty po kolei z przetasowanej listy
            // Jeśli mamy więcej par niż rodzajów herbat, lista zapętli się i zacznie od nowa
            GameObject chosen = availablePrefabs[i % availablePrefabs.Count];
            cardsToSpawn.Add(chosen);
            cardsToSpawn.Add(chosen);
        }

        // 6. Tasujemy finalną listę kart, aby pary nie leżały obok siebie (Algorytm Fisher-Yates)
        for (int i = 0; i < cardsToSpawn.Count; i++)
        {
            GameObject temp = cardsToSpawn[i];
            int randomIndex = Random.Range(i, cardsToSpawn.Count);
            cardsToSpawn[i] = cardsToSpawn[randomIndex];
            cardsToSpawn[randomIndex] = temp;
        }

        // 7. Instancjonowanie kart
        foreach (GameObject prefab in cardsToSpawn)
        {
            // 'false' zapewnia, że karty nie zmienią skali przy dodawaniu do siatki
            Instantiate(prefab, gridContainer, false);
        }
    }
}