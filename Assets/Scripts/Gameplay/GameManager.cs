using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Players")]
    public List<Player> players = new List<Player>();
    public int currentPlayerIndex = 0;

    public Player CurrentPlayer => players[currentPlayerIndex];

    private void Awake()
    {
        // Singleton (na studia OK)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitPlayers();
    }

    private void InitPlayers()
    {
        players.Clear();

        // SOLO, ale gotowe pod PVP
        players.Add(new Player(0));
        // players.Add(new Player(1)); // <- PVP później

        currentPlayerIndex = 0;

        Debug.Log("Players initialized. Current player: " + CurrentPlayer.id);
    }

    public void OnMatch()
    {
        CurrentPlayer.AddPoint();
        Debug.Log($"Player {CurrentPlayer.id} MATCH! Score: {CurrentPlayer.score}, Combo: {CurrentPlayer.combo}");
    }

    public void OnMismatch()
    {
        CurrentPlayer.ResetCombo();
        SwitchTurn();
    }

    private void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Debug.Log("Turn switched. Current player: " + CurrentPlayer.id);
    }
}
