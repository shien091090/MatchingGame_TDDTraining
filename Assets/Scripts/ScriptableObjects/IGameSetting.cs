namespace GameCore
{
    public interface IGameSetting
    {
        int GetSuccessIncreasePoint { get; }
        int GetFailIncreasePoint { get; }
        int GetPairCount { get; }
        float GetCardDelayCoverTimes { get; }
        float NormalFrozeTimes { get; }
        float NotMatchFrozeTimes { get; }
    }
}