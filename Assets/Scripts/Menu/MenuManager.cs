using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nickInput; // To pole jest używane TYLKO w menu głównym (scena MB)

    public void StartGame()
    {
        // Sprawdzamy nick
        string nick = nickInput.text;
        if (string.IsNullOrWhiteSpace(nick)) nick = "Bezimienny";

        PlayerPrefs.SetString("CurrentPlayerNick", nick);
        SceneManager.LoadScene("WK");
    }

    // TA FUNKCJA JEST DLA PRZYCISKU NA SCENIE OŚWIECENIE
    public void BackToMenu()
    {
        SceneManager.LoadScene("MB");
    }

    public void ExitGame()
    {
        // To wyłączy grę po zbudowaniu do .exe
        Application.Quit();

        // DODAJ TO: To wyłączy tryb "Play" wewnątrz edytora Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}