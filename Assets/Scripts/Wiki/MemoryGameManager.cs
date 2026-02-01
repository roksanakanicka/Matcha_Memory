using UnityEngine;
using System.Collections;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager instance;


    public MatchEffectPlayer matchEffectPrefab; // PRZEPISZ TUTAJ PREFAB Z FOLDERU ASSETS
    public TeaTemperature teaTimer;
    public RectTransform gridContainer;        // PRZECIĄGNIJ TUTAJ PANEL 'CardGrid'

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
        //... (poprzedni kod dźwięku i temperatury)...

        if (matchEffectPrefab != null && gridContainer != null)
        {
            Transform canvasTransform = gridContainer.parent;

            // Spawnowanie efektów
            GameObject eff1 = Instantiate(matchEffectPrefab.gameObject, canvasTransform);
            GameObject eff2 = Instantiate(matchEffectPrefab.gameObject, canvasTransform);

            // KROK A: Ustawiamy pozycję globalną (World Position) na środek karty
            eff1.transform.position = firstCard.transform.position;
            eff2.transform.position = secondCard.transform.position;

            // KROK B: Jeśli efekt ma RectTransform, wymuszamy wyzerowanie pozycji lokalnej względem punktu zaczepienia
            RectTransform rt1 = eff1.GetComponent<RectTransform>();
            RectTransform rt2 = eff2.GetComponent<RectTransform>();

            if (rt1 != null)
            {
                // To usuwa wszelkie "przesunięcia w prawo" zapisane w prefabie
                rt1.anchoredPosition = canvasTransform.InverseTransformPoint(firstCard.transform.position);
                // Alternatywnie, jeśli powyższe wydaje się skomplikowane, upewnij się po prostu, 
                // że w prefabie Pos X i Pos Y są równe 0.
            }

            // KROK C: Ustawienie Z (żeby efekt był przed kartą)
            eff1.transform.localPosition = new Vector3(eff1.transform.localPosition.x, eff1.transform.localPosition.y, -10f);
            eff2.transform.localPosition = new Vector3(eff2.transform.localPosition.x, eff2.transform.localPosition.y, -10f);

            Destroy(eff1, 2f);
            Destroy(eff2, 2f);
        }
    }

    void HandleMismatch()
    {
        comboCount = 0;
        firstCard.ExecuteFlipAnimation();
        secondCard.ExecuteFlipAnimation();
    }
}