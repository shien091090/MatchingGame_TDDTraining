using System;

public class Card
{
    public int number;
    public event Action<bool> OnSwitchCoverState;
    public bool IsCovered { get; private set; }
    public int GetPattern { get; }

    public Card(int patternNumber, int number)
    {
        GetPattern = patternNumber;
        this.number = number;
        IsCovered = true;
    }

    public void Flap()
    {
        IsCovered = false;
        OnSwitchCoverState?.Invoke(false);
    }

    public void Cover()
    {
        IsCovered = true;
        OnSwitchCoverState?.Invoke(true);
    }
}