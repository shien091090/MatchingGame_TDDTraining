using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;

namespace GameCore
{
    public class SwitchCoverStateEvent : IArchitectureEvent, IAudioTriggerEvent
    {
        public string PreSetParamName { get; set; }
        public float PreSetParamValue { get; set; }
        public bool IsCover { get; }
        public int CardNumber { get; }

        public SwitchCoverStateEvent(int cardNumber, bool isCover)
        {
            IsCover = isCover;
            CardNumber = cardNumber;
        }
    }
}