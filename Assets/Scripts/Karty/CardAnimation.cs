using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Wymagane do wykrywania myszki

public class CardAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float scaleFactor = 1.1f;
    public float animationSpeed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale;


    public float flipSpeed = 5f;
    public GameObject cardFront; // Ikona/przód karty
    public GameObject cardBack;  // Tło/tył karty
    private bool isFlipping = false;
    private bool faceUp = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        // Na starcie przód karty jest ukryty
        cardFront.SetActive(false);
    }

    void Update()
    {
        // Gładkie skalowanie (Hover)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // Wykrywanie najechania myszką
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    // Funkcja do wywołania po kliknięciu
    public void StartFlip()
    {
        if (!isFlipping)
            StartCoroutine(FlipCoroutine());
    }

    private System.Collections.IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        float time = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(0, 90, 0);

        // Pierwsza połowa obrotu (do 90 stopni)
        while (time < 1f)
        {
            time += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
        }

        // Zamiana stron w połowie obrotu
        faceUp = !faceUp;
        cardFront.SetActive(faceUp);
        cardBack.SetActive(!faceUp);

        // Druga połowa obrotu (z powrotem do 0/180)
        time = 0f;
        startRotation = transform.rotation;
        endRotation = transform.rotation * Quaternion.Euler(0, -90, 0);

        while (time < 1f)
        {
            time += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
        }

        isFlipping = false;
    }
}
