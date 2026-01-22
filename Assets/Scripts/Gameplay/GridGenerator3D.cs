using System.Collections.Generic;
using UnityEngine;

public class GridGenerator3D : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 4;
    public int columns = 4;
    public float spacingX = 1.2f;
    public float spacingZ = 1.6f;

    [Header("References")]
    public Card3D cardPrefab;
    public Transform parentTransform;

    [Header("Materials")]
    public Material backMaterial;
    public List<Material> faceMaterials = new List<Material>();

    private readonly List<int> cardIDs = new List<int>();

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (cardPrefab == null || parentTransform == null || backMaterial == null)
        {
            Debug.LogError("GridGenerator3D: Missing references (cardPrefab / parentTransform / backMaterial).");
            return;
        }

        int totalCards = rows * columns;
        if (totalCards % 2 != 0)
        {
            Debug.LogError("GridGenerator3D: rows*columns must be even.");
            return;
        }

        int pairs = totalCards / 2;

        if (faceMaterials.Count < pairs)
        {
            Debug.LogError($"GridGenerator3D: Not enough face materials. Need at least {pairs}, have {faceMaterials.Count}.");
            return;
        }

        // Clear old cards
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
            Destroy(parentTransform.GetChild(i).gameObject);

        // Build ID list (pairs)
        cardIDs.Clear();
        for (int i = 0; i < pairs; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }
        Shuffle(cardIDs);

        // Center grid around parent
        float width = (columns - 1) * spacingX;
        float depth = (rows - 1) * spacingZ;
        Vector3 origin = parentTransform.position - new Vector3(width / 2f, 0f, -depth / 2f);

        for (int i = 0; i < totalCards; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 pos = origin + new Vector3(col * spacingX, 0f, -row * spacingZ);

            Card3D card = Instantiate(cardPrefab, pos, Quaternion.Euler(90f, 0f, 0f), parentTransform);
            int id = cardIDs[i];

            card.Setup(
                id,
                faceMaterials[id],
                backMaterial,
                AbilityType.None
            );
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
