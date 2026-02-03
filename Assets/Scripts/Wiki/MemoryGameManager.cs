using UnityEngine;
using UnityEngine.UI;    // Obsługuje typ Image
using TMPro;             // Obsługuje teksty TextMeshPro
using DG.Tweening;       // Obsługuje animacje DOTween
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;


    public GridGenerator gridGenerator;
    public TeaTemperature teaTimer;

    [Header("Efekty i UI")]
    public MatchEffectPlayer matchEffectPrefab;
    public Camera uiCamera;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Efekt Combo Vignette")]
    public Image comboVignette; // TU PRZECIĄGNIJ OBIEKT Z HIERARCHII
    public float maxFlashAlpha = 0.7f;

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
        if (gridGenerator == null)
        {
            Debug.LogError("BŁĄD: Nie przypisano GridGeneratora!");
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

        if (firstCard == null) firstCard = card;
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

        // WYWOŁANIE ROZBŁYSKU WINIETY
        TriggerComboFlash();

        UpdateUI();

        if (matchEffectPrefab != null) SpawnVFXAtCard(firstCard);

        StartCoroutine(HideCardsWithCanvasGroup(firstCard, secondCard));

        if (pairsFound >= totalPairs)
        {
            boardLevel++;
            StartCoroutine(PrepareNextBoard());
        }
    }

    void TriggerComboFlash()
    {
        if (comboVignette == null) return;

        // Im większe combo, tym silniejszy błysk (do limitu maxFlashAlpha)
        float intensity = Mathf.Min(0.1f * comboCount, maxFlashAlpha);

        comboVignette.DOKill(); // Zatrzymaj poprzedni błysk

        // Animacja rozbłysku i powolnego zanikania 
        Sequence s = DOTween.Sequence();
        s.Append(comboVignette.DOFade(intensity, 0.05f));
        s.Append(comboVignette.DOFade(0f, 0.4f));
    }

    public IEnumerator PrepareNextBoard()
    {
        isProcessing = true;
        yield return new WaitForSeconds(1.0f);
        gridGenerator.GenerateGrid();
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
        else { c1.gameObject.SetActive(false); c2.gameObject.SetActive(false); }
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