using GameCore;
using SNShien.Common.ArchitectureTools;

public class Card
{
    private ArchitectureEventHandler eventHandler;
    public readonly int number;

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
    }

    public void Cover()
    {
        IsCovered = true;
    }
}