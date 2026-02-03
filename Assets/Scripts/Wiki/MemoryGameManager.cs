using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;


    public GridGenerator gridGenerator; // To pole MUSI być przypisane w Inspectorze!
    public TeaTemperature teaTimer;

    [Header("Efekty i UI")]
    public MatchEffectPlayer matchEffectPrefab;
    public Camera uiCamera;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Ustawienia Arcade")]
    public int pointsPerMatch = 100;
    public float baseReheatAmount = 12f;
    public float waitTimeBeforeFlip = 0.8f;
    public float speedIncreaseFactor = 0.2f;

    private CardAnimation firstCard;
    private CardAnimation secondCard;
    private bool isProcessing = false;
    private int currentScore = 0;
    public int comboCount = 0;

    private int pairsFound = 0;
    private int totalPairs;
    private int boardLevel = 1;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Sprawdzamy, czy przypisano generator w Inspectorze 
        if (gridGenerator == null)
        {
            Debug.LogError("BŁĄD: Nie przypisano GridGeneratora do MemoryGameManager!");
            return;
        }
        StartCoroutine(PrepareNextBoard());
    }

    void Update()
    {
        if (teaTimer != null && teaTimer.GetCurrentTemperature() <= 0)
        {
            GameOver();
        }
    }

    void InitializeBoard()
    {
        // Pobieramy liczbę par bezpośrednio z ustawień generatora [1]
        totalPairs = (gridGenerator.rows * gridGenerator.columns) / 2;
        pairsFound = 0;

        if (teaTimer != null)
        {
            float newMultiplier = 1f + ((boardLevel - 1) * speedIncreaseFactor);
            teaTimer.SetSpeedMultiplier(newMultiplier);
        }
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
        pairsFound++;

        if (AudioManager.instance != null) AudioManager.instance.PlayMatch();
        if (teaTimer != null) teaTimer.ReheatTea(baseReheatAmount * (1f + (comboCount / 5f)));

        UpdateUI();

        if (matchEffectPrefab != null)
        {
            SpawnVFXAtCard(firstCard);
            SpawnVFXAtCard(secondCard);
        }

        StartCoroutine(HideCardsWithCanvasGroup(firstCard, secondCard));

        if (pairsFound >= totalPairs)
        {
            boardLevel++;
            StartCoroutine(PrepareNextBoard());
        }
    }

    public IEnumerator PrepareNextBoard()
    {
        isProcessing = true;
        yield return new WaitForSeconds(1.0f);

        // Wywołujemy generator (on sam wyczyści starą planszę) [1, 2]
        gridGenerator.GenerateGrid();

        // Czekamy klatkę, aby obiekty zdążyły się zainicjalizować
        yield return new WaitForEndOfFrame();

        InitializeBoard();
        isProcessing = false;

        if (boardLevel > 1 && comboText != null) comboText.text = "POZIOM " + boardLevel;
    }

    void GameOver()
    {
        PlayerPrefs.SetFloat("LastScore", currentScore);
        SceneManager.LoadScene("Oswiecenie");
    }

    private void SpawnVFXAtCard(CardAnimation card)
    {
        // Korzystamy z gridContainer z generatora, aby nie dublować referencji
        RectTransform canvasRect = gridGenerator.gridContainer.parent as RectTransform;
        GameObject eff = Instantiate(matchEffectPrefab.gameObject, canvasRect);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            uiCamera.WorldToScreenPoint(card.transform.position), uiCamera, out localPoint);
        eff.GetComponent<RectTransform>().anchoredPosition = localPoint;
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