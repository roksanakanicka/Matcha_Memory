using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("DANE (z Assets)")]
    public CardData daneTejKarty;

    [Header("WYŚWIETLACZE (Elementy UI)")]
    public TextMeshProUGUI poleTekstowe;
    public TextMeshProUGUI poleOpisu;
    public Image poleObrazka;

    // To sprawia, że karta odświeża się sama w edytorze po wrzuceniu danych
    private void OnValidate()
    {
        if (daneTejKarty != null) OdswiezWyglad();
    }

    void Start()
    {
        OdswiezWyglad();
    }

    public void OdswiezWyglad()
    {
        if (daneTejKarty == null) return;

        if (poleTekstowe != null) poleTekstowe.text = daneTejKarty.nazwaKarty;
        if (poleOpisu != null) poleOpisu.text = daneTejKarty.opisEfektu;
        if (poleObrazka != null) poleObrazka.sprite = daneTejKarty.ilustracjaKarty;
    }
}