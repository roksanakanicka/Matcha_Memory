using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nickInput;

    public void StartGame()
    {
        // Sprawdzamy co wpisał gracz
        string nick = nickInput.text;

        if (string.IsNullOrWhiteSpace(nick))
        {
            nick = "Bezimienny";
        }

        // POPRAWIONO: Klucz zmieniony na "CurrentPlayerNick" dla spójności z rankingiem
        PlayerPrefs.SetString("CurrentPlayerNick", nick);

        SceneManager.LoadScene("WK");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}