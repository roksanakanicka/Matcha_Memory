using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // DODANE: Wymagane do zmiany scen

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;

    public MatchEffectPlayer matchEffectPrefab;
    public TeaTemperature teaTimer;
    public RectTransform gridContainer;
    public Camera uiCamera;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Ustawienia")]
    public int pointsPerMatch = 100;
    public float baseReheatAmount = 10f;
    public float waitTimeBeforeFlip = 0.8f;

    private CardAnimation firstCard;
    private CardAnimation secondCard;
    private bool isProcessing = false;
    private int currentScore = 0;
    public int comboCount = 0;

    // DODANE: Zmienna do liczenia zebranych par
    private int pairsFound = 0;
    private int totalPairs;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Obliczamy ile par jest na stole (liczba dzieci w gridzie podzielona na 2)
        totalPairs = gridContainer.childCount / 2;
    }

    public void OnCardClicked(CardAnimation card)
    {
        if (isProcessing || card.faceUp || card == firstCard) return;

        card.ExecuteFlipAnimation();

        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isProcessing = true;
        yield return new WaitForSeconds(waitTimeBeforeFlip);

        int id1 = firstCard.GetComponent<CardsData>().cardID;
        int id2 = secondCard.GetComponent<CardsData>().cardID;

        if (id1 == id2) HandleMatch();
        else HandleMismatch();

        firstCard = null;
        secondCard = null;
        isProcessing = false;
    }

    void HandleMatch()
    {
        comboCount++;
        currentScore += (int)(pointsPerMatch * (1f + (comboCount / 10f)));
        pairsFound++; // DODANE: Zwiększamy licznik zebranych par

        if (AudioManager.instance != null) AudioManager.instance.PlayMatch();
        if (teaTimer != null) teaTimer.ReheatTea(baseReheatAmount * (1f + (comboCount / 5f)));

        UpdateUI();

        if (matchEffectPrefab != null && gridContainer != null && uiCamera != null)
        {
            SpawnVFXAtCard(firstCard);
            SpawnVFXAtCard(secondCard);
        }

        StartCoroutine(HideCardsWithCanvasGroup(firstCard, secondCard));

        // DODANE: Sprawdzanie czy to była ostatnia para
        if (pairsFound >= totalPairs)
        {
            GameOver(true);
        }
    }

    // DODANE: Funkcja kończąca grę i zapisująca wynik
    void GameOver(bool win)
    {
        if (win)
        {
            // Zapisujemy ostateczny wynik do pamięci
            PlayerPrefs.SetFloat("LastScore", currentScore);

            // Przechodzimy do sceny rankingu
            SceneManager.LoadScene("Oswiecenie");
        }
    }

    private void SpawnVFXAtCard(CardAnimation card)
    {
        RectTransform canvasRect = gridContainer.parent as RectTransform;
        GameObject eff = Instantiate(matchEffectPrefab.gameObject, canvasRect);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            uiCamera.WorldToScreenPoint(card.transform.position), uiCamera, out localPoint);

        eff.GetComponent<RectTransform>().anchoredPosition = localPoint;
        eff.transform.localScale = Vector3.one;
        eff.transform.localPosition = new Vector3(eff.transform.localPosition.x, eff.transform.localPosition.y, -2f);
        Destroy(eff, 2f);
    }

    private IEnumerator HideCardsWithCanvasGroup(CardAnimation c1, CardAnimation c2)
    {
        yield return new WaitForSeconds(0.5f);
        CanvasGroup cg1 = c1.GetComponent<CanvasGroup>();
        CanvasGroup cg2 = c2.GetComponent<CanvasGroup>();

        if (cg1 != null && cg2 != null)
        {
            cg1.alpha = 0; cg1.blocksRaycasts = false;
            cg2.alpha = 0; cg2.blocksRaycasts = false;
        }
        else
        {
            c1.gameObject.SetActive(false);
            c2.gameObject.SetActive(false);
        }
    }

    void HandleMismatch()
    {
        comboCount = 0;
        UpdateUI();
        firstCard.ExecuteFlipAnimation();
        secondCard.ExecuteFlipAnimation();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Wynik: " + currentScore;
        if (comboText != null) comboText.text = comboCount > 1 ? "Combo x" + comboCount : "";
    }
}