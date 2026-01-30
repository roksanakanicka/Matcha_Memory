using UnityEngine;

public class CardTester : MonoBehaviour
{
    public CardDisplay[] wszystkieKarty;

    [ContextMenu("Odśwież Galerię")]
    public void OdswiezWszystkie()
    {
        foreach (var karta in wszystkieKarty)
        {
            karta.OdswiezWyglad();
        }
    }
}