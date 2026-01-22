using UnityEngine;

public class Card3D : MonoBehaviour
{
    [Header("Card Data")]
    public int cardID;
    public AbilityType abilityType = AbilityType.None;

    [Header("Visuals")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material backMaterial;
    [SerializeField] private Material faceMaterial;

    public bool IsFaceUp { get; private set; }
    public bool IsMatched { get; private set; }

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Setup(int id, Material face, Material back, AbilityType ability)
    {
        cardID = id;
        faceMaterial = face;
        backMaterial = back;
        abilityType = ability;

        IsMatched = false;
        SetFaceUp(false);
    }

    public void SetMatched(bool matched)
    {
        IsMatched = matched;
    }

    public void SetFaceUp(bool faceUp)
    {
        IsFaceUp = faceUp;
        if (meshRenderer != null)
            meshRenderer.material = IsFaceUp ? faceMaterial : backMaterial;
    }
}
