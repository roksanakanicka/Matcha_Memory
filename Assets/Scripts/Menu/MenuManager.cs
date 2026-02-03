using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nickInput;

    public void StartGame()
    {
        // Sprawdzamy co wpisał gracz
        // To zadziała TYLKO w scenie menu (MB), gdzie jest pole nickInput
        string nick = nickInput.text;

        if (string.IsNullOrWhiteSpace(nick))
        {
            nick = "Bezimienny";
        }

        // Zapisujemy nick dla rankingu
        PlayerPrefs.SetString("CurrentPlayerNick", nick);

        SceneManager.LoadScene("WK");
    }

    // TA FUNKCJA NAPRAWI BŁĄD NA SCENIE OŚWIECENIE
    public void BackToMenu()
    {
        // Po prostu ładujemy menu, nie sprawdzając pola tekstowego
        SceneManager.LoadScene("MB");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}