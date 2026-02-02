using UnityEngine;
using System.Collections;
using TMPro; // Wymagane do obsługi wyświetlania wyniku i combo

public class MemoryGameManager : MonoBehaviour
{
    // Singleton - pozwala innym skryptom na łatwy dostęp do Managera
    public static MemoryGameManager instance;


    public MatchEffectPlayer matchEffectPrefab; // Prefab VFX liści i pary
    public TeaTemperature teaTimer;            // Skrypt termometru
    public RectTransform gridContainer;        // Panel 'CardGrid'
    public Camera uiCamera;                   // Twoja UICamera


    public TextMeshProUGUI scoreText;         // Tekst wyniku
    public TextMeshProUGUI comboText;         // Tekst combo

    [Header("Ustawienia logiki i punktacji")]
    public int pointsPerMatch = 100;          // Bazowe punkty za parę
    public float baseReheatAmount = 10f;      // Ile stopni wraca do herbaty
    public float waitTimeBeforeFlip = 0.8f;   // Czas na podejrzenie kart przed zakryciem

    // Zmienne prywatne (stan gry)
    private CardAnimation firstCard;
    private CardAnimation secondCard;
    private bool isProcessing = false;
    private int currentScore = 0;

    [HideInInspector]
    public int comboCount = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void OnCardClicked(CardAnimation card)
    {
        // Blokady zabezpieczające przed błędami i klikaniem odkrytych kart
        if (isProcessing || card.faceUp || card == firstCard)
            return;

        // Manager wydaje zgodę na animację obrotu
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

        // Pobieramy ID z obu kart (identyfikacja pary)
        int id1 = firstCard.GetComponent<CardsData>().cardID;
        int id2 = secondCard.GetComponent<CardsData>().cardID;

        if (id1 == id2)
        {
            HandleMatch();
        }
        else
        {
            HandleMismatch();
        }

        // Resetowanie wyboru i odblokowanie klikania
        firstCard = null;
        secondCard = null;
        isProcessing = false;
    }

    void HandleMatch()
    {
        comboCount++;

        // 1. Obliczanie punktacji: P = P_base * (1 + C/10)
        int matchPoints = (int)(pointsPerMatch * (1f + (comboCount / 10f)));
        currentScore += matchPoints;

        // 2. Dźwięk sukcesu
        if (AudioManager.instance != null) AudioManager.instance.PlayMatch();

        // 3. Podgrzewanie herbaty z bonusem za combo: Gain = Base * (1 + C/5)
        float bonusMultiplier = 1f + (comboCount / 5f);
        if (teaTimer != null) teaTimer.ReheatTea(baseReheatAmount * bonusMultiplier);

        // 4. Centrowanie efektu VFX (RectTransformUtility)
        if (matchEffectPrefab != null && gridContainer != null && uiCamera != null)
        {
            RectTransform canvasRect = gridContainer.parent as RectTransform;
            GameObject eff1 = Instantiate(matchEffectPrefab.gameObject, canvasRect);
            GameObject eff2 = Instantiate(matchEffectPrefab.gameObject, canvasRect);

            // Matematyczne centrowanie na kartach
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                uiCamera.WorldToScreenPoint(firstCard.transform.position), uiCamera, out localPoint);
            eff1.GetComponent<RectTransform>().anchoredPosition = localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                uiCamera.WorldToScreenPoint(secondCard.transform.position), uiCamera, out localPoint);
            eff2.GetComponent<RectTransform>().anchoredPosition = localPoint;

            eff1.transform.localScale = Vector3.one;
            eff2.transform.localScale = Vector3.one;

            Destroy(eff1, 2f);
            Destroy(eff2, 2f);
        }

        // 5. Wyłączenie kart z dalszej gry
        firstCard.GetComponent<UnityEngine.UI.Button>().interactable = false;
        secondCard.GetComponent<UnityEngine.UI.Button>().interactable = false;

        UpdateScoreUI();
    }

    void HandleMismatch()
    {
        comboCount = 0; // Reset combo przy pomyłce
        firstCard.ExecuteFlipAnimation();
        secondCard.ExecuteFlipAnimation();
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Wynik: " + currentScore;
        if (comboText != null)
        {
            comboText.text = comboCount > 1 ? "Combo x" + comboCount : "";
        }
    }
}