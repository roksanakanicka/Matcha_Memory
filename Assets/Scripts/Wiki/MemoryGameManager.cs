using UnityEngine;
using System.Collections;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;


    public MatchEffectPlayer matchEffectPrefab;
    public TeaTemperature teaTimer;
    public RectTransform gridContainer;
    public Camera uiCamera; // <--- TEJ LINII BRAKOWAŁO (Naprawia błąd CS0103)

    [Header("Ustawienia logiki")]
    public float baseReheatAmount = 10f;
    public float waitTimeBeforeFlip = 0.8f;

    private CardAnimation firstCard;
    private CardAnimation secondCard;
    private bool isProcessing = false;

    [HideInInspector]
    public int comboCount = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void OnCardClicked(CardAnimation card)
    {
        if (isProcessing || card.faceUp || card == firstCard)
            return;

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

        if (id1 == id2)
        {
            HandleMatch();
        }
        else
        {
            HandleMismatch();
        }

        firstCard = null;
        secondCard = null;
        isProcessing = false;
    }

    void HandleMatch()
    {
        comboCount++;
        if (AudioManager.instance != null) AudioManager.instance.PlayMatch();

        float bonus = 1f + (comboCount / 5f);
        if (teaTimer != null) teaTimer.ReheatTea(baseReheatAmount * bonus);

        if (matchEffectPrefab != null && gridContainer != null && uiCamera != null)
        {
            // 1. Pobieramy RectTransform Canvasa (rodzica siatki)
            RectTransform canvasRect = gridContainer.parent as RectTransform;

            // 2. Tworzymy kopie efektów (Prefab VFX) jako dzieci Canvasa
            GameObject eff1 = Instantiate(matchEffectPrefab.gameObject, canvasRect);
            GameObject eff2 = Instantiate(matchEffectPrefab.gameObject, canvasRect);

            // --- MATEMATYCZNE WYŚRODKOWANIE (RectTransformUtility) ---
            // Przeliczamy pozycję karty na punkt lokalny na Canvasie
            Vector2 localPoint1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                uiCamera.WorldToScreenPoint(firstCard.transform.position), uiCamera, out localPoint1);
            eff1.GetComponent<RectTransform>().anchoredPosition = localPoint1;

            Vector2 localPoint2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                uiCamera.WorldToScreenPoint(secondCard.transform.position), uiCamera, out localPoint2);
            eff2.GetComponent<RectTransform>().anchoredPosition = localPoint2;

            // 3. Reset skali i przesunięcie Z (by liście były przed kartą)
            eff1.transform.localScale = Vector3.one;
            eff2.transform.localScale = Vector3.one;
            eff1.transform.localPosition = new Vector3(eff1.transform.localPosition.x, eff1.transform.localPosition.y, -2f);
            eff2.transform.localPosition = new Vector3(eff2.transform.localPosition.x, eff2.transform.localPosition.y, -2f);

            Destroy(eff1, 2f);
            Destroy(eff2, 2f);
        }

        firstCard.GetComponent<UnityEngine.UI.Button>().interactable = false;
        secondCard.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    void HandleMismatch()
    {
        comboCount = 0;
        firstCard.ExecuteFlipAnimation();
        secondCard.ExecuteFlipAnimation();
    }
}