using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText;
    private const int rankingSize = 15;

    void Start()
    {
        ZapiszNowyWynik();
        WyswietlRanking();
    }

    void ZapiszNowyWynik()
    {
        // Pobieramy nick zapisany w MenuManagerze
        string nick = PlayerPrefs.GetString("CurrentPlayerNick", "Gracz");
        float wynik = PlayerPrefs.GetFloat("LastScore", 0);

        List<KeyValuePair<string, float>> wyniki = new List<KeyValuePair<string, float>>();

        for (int i = 0; i < rankingSize; i++)
        {
            if (PlayerPrefs.HasKey("RankScore" + i))
            {
                string oldNick = PlayerPrefs.GetString("RankNick" + i, "-");
                float oldScore = PlayerPrefs.GetFloat("RankScore" + i, 0);
                wyniki.Add(new KeyValuePair<string, float>(oldNick, oldScore));
            }
        }

        if (wynik > 0 || nick != "Gracz")
        {
            wyniki.Add(new KeyValuePair<string, float>(nick, wynik));
        }

        wyniki.Sort((x, y) => y.Value.CompareTo(x.Value));

        for (int i = 0; i < Mathf.Min(wyniki.Count, rankingSize); i++)
        {
            PlayerPrefs.SetString("RankNick" + i, wyniki[i].Key);
            PlayerPrefs.SetFloat("RankScore" + i, wyniki[i].Value);
        }

        PlayerPrefs.SetFloat("LastScore", 0);
    }

    void WyswietlRanking()
    {
        rankingText.text = "<size=120%>RANKING TOP 30</size>\n\n";
        for (int i = 0; i < rankingSize; i++)
        {
            if (PlayerPrefs.HasKey("RankScore" + i))
            {
                string nick = PlayerPrefs.GetString("RankNick" + i, "---");
                float score = PlayerPrefs.GetFloat("RankScore" + i, 0);
                // Używamy prostego formatowania tekstowego zamiast <pos>
                rankingText.text += $"{i + 1}. {nick} ....... {score} pkt\n";
            }
            else
            {
                rankingText.text += $"<color=#777777>{i + 1}. ---</color>\n";
            }
        }
    }
}