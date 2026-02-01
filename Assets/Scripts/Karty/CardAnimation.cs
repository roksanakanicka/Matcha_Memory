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

    [HideInInspector]
    public bool isFlipping = false;
    [HideInInspector]
    public bool faceUp = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Ustawienie początkowej widoczności stron
        cardFront.SetActive(faceUp);
        cardBack.SetActive(!faceUp);
    }

    void Update()
    {
        // Gładkie skalowanie przy najechaniu myszką (Hover)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // --- FUNKCJA 1: Wywoływana przez Button (UI) ---
    public void OnCardButtonClick()
    {
        // Sprawdzamy tylko, czy karta już się nie obraca
        if (!isFlipping)
        {
            // Karta mówi do Managera: "Hej, kliknięto mnie! Co mam robić?"
            MemoryGameManager.instance.OnCardClicked(this);
        }
    }

    // --- FUNKCJA 2: Wywoływana przez Manager (Logika) ---
    public void ExecuteFlipAnimation()
    {
        // Manager dał zielone światło, więc odpalamy animację coroutiną
        if (!isFlipping)
        {
            StartCoroutine(FlipCoroutine());
        }
    }

    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;

        // Dźwięk przez AudioManagera (Singleton)
        if (AudioManager.instance != null) AudioManager.instance.PlayFlip();

        // KROK 1: Obrót do 90 stopni (bokiem do gracza)
        float time = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion midRotation = transform.rotation * Quaternion.Euler(0, 90, 0);

        while (time < 1f)
        {
            time += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, midRotation, time);
            yield return null; // Czekaj na następną klatkę
        }

        // KROK 2: Zamiana grafiki w połowie obrotu
        faceUp = !faceUp;
        cardFront.SetActive(faceUp);
        cardBack.SetActive(!faceUp);

        // KROK 3: Dokończenie obrotu o kolejne 90 stopni
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

    // Obsługa Hover (najechanie myszką)
    public void OnPointerEnter(PointerEventData eventData) => targetScale = originalScale * scaleFactor;
    public void OnPointerExit(PointerEventData eventData) => targetScale = originalScale;
}