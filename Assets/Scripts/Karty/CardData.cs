using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta", menuName = "Karta/DaneKarty")]
public class CardData : ScriptableObject
{
    public string nazwaKarty;
    public Sprite ilustracjaKarty;
    [TextArea(3, 10)]
    public string opisEfektu;
}