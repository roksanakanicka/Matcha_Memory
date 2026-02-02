using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CardAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ustawienia Hover")]
    public float scaleFactor = 1.1f;
    public float animationSpeed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    [Header("Ustawienia Flip")]
    public float flipSpeed = 5f;
    public GameObject cardFront;
    public GameObject cardBack;

    [HideInInspector] public bool isFlipping = false;
    [HideInInspector] public bool faceUp = false;

    private CanvasGroup canvasGroup;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        canvasGroup = GetComponent<CanvasGroup>();

        // Początkowy stan: karta zakryta
        cardFront.SetActive(faceUp);
        cardBack.SetActive(!faceUp);
    }

    void Update()
    {
        // Gładkie skalowanie (Hover)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // --- TYLKO DLA BUTTONA (OnClick) ---
    public void OnCardButtonClick()
    {
        // Jeśli karta zniknęła (alpha 0) lub trwa obrót - ignoruj
        if ((canvasGroup != null && canvasGroup.alpha <= 0) || isFlipping) return;

        // Wysyłamy prośbę do Managera
        MemoryGameManager.instance.OnCardClicked(this);
    }

    // --- TYLKO DLA MANAGERA ---
    public void ExecuteFlipAnimation()
    {
        if (!isFlipping) StartCoroutine(FlipCoroutine());
    }

    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        if (AudioManager.instance != null) AudioManager.instance.PlayFlip();

        float time = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion midRotation = transform.rotation * Quaternion.Euler(0, 90, 0);

        while (time < 1f)
        {
            time += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, midRotation, time);
            yield return null;
        }

        faceUp = !faceUp;
        cardFront.SetActive(faceUp);
        cardBack.SetActive(!faceUp);

        time = 0f;
        startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(0, -90, 0);

        while (time < 1f)
        {
            time += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
        }
        isFlipping = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup != null && canvasGroup.alpha <= 0) return;
        targetScale = originalScale * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData) => targetScale = originalScale;
}