using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nickInput; // To pole jest używane TYLKO w menu głównym

    public void StartGame()
    {
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
        Application.Quit();
    }
}