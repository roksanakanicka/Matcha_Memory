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
    public List<GameObject> cardPrefabs;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // 1. Konfiguracja GridLayoutGroup
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            float newWidth = baseCellSize.x * cardScale;
            float newHeight = baseCellSize.y * cardScale;
            gridLayout.cellSize = new Vector2(newWidth, newHeight);

            float horizontalSpacing = newWidth * spacingPercentage;
            float verticalSpacing = newHeight * spacingPercentage;
            gridLayout.spacing = new Vector2(horizontalSpacing, verticalSpacing);

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
        }

        // 2. Czyszczenie starych kart
        foreach (Transform child in gridContainer) { Destroy(child.gameObject); }

        // 3. Obliczanie par
        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Suma pól siatki musi być parzysta!");
            return;
        }

        List<GameObject> cardsToSpawn = new List<GameObject>();
        for (int i = 0; i < totalCards / 2; i++)
        {
            // FIX: Pobieramy losowy indeks z dostępnych prefabów
            int randomIndex = Random.Range(0, cardPrefabs.Count);
            GameObject chosenPrefab = cardPrefabs[randomIndex];

            // Dodajemy parę tego samego prefaba
            cardsToSpawn.Add(chosenPrefab);
            cardsToSpawn.Add(chosenPrefab);
        }

        // 4. Tasowanie Fisher-Yates
        for (int i = 0; i < cardsToSpawn.Count; i++)
        {
            GameObject temp = cardsToSpawn[i];
            int randomIndex = Random.Range(i, cardsToSpawn.Count);
            cardsToSpawn[i] = cardsToSpawn[randomIndex];
            cardsToSpawn[randomIndex] = temp;
        }

        // 5. Tworzenie kart na scenie
        foreach (GameObject prefab in cardsToSpawn)
        {
            Instantiate(prefab, gridContainer, false);
        }
    }
}