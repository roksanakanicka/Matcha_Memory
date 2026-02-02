using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public TMP_InputField nickInput;

    public void Zagraj()
    {
        string nick = nickInput.text;
        if (string.IsNullOrEmpty(nick)) nick = "Gracz"; // Domyślny nick

        // Zapisujemy nick tymczasowo, żeby użyć go po wygranej
        PlayerPrefs.SetString("CurrentNick", nick);
        SceneManager.LoadScene("WK"); // Twoja scena z grą
    }
}