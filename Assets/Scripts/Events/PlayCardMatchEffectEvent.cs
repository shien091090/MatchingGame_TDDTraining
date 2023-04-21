using System.Collections.Generic;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class PlayCardMatchEffectEvent : IArchitectureEvent, IAudioTriggerEvent
    {
        public string PreSetParamName { get; set; }
        public float PreSetParamValue { get; set; }
        private List<int> MatchCardNumbers { get; }

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