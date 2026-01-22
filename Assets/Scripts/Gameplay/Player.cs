using System;

[Serializable]
public class Player
{
    public int id;
    public int score;
    public int combo;

    public Player(int id)
    {
        this.id = id;
        score = 0;
        combo = 0;
    }

    public void AddPoint()
    {
        score++;
        combo++;
    }

    public void ResetCombo()
    {
        combo = 0;
    }
}
