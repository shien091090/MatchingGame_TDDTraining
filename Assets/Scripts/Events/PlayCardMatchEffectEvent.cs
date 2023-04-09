using System.Collections.Generic;
using SNShien.Common.ArchitectureTools;

namespace GameCore
{
    public class PlayCardMatchEffectEvent : IArchitectureEvent
    {
        public List<int> MatchCardNumbers { get; }

        public PlayCardMatchEffectEvent(List<int> numbers)
        {
            MatchCardNumbers = numbers;
        }

        public bool CheckIsMatchNumber(int number)
        {
            return MatchCardNumbers.Contains(number);
        }
    }
}