using SNShien.Common.ArchitectureTools;

namespace GameCore
{
    public class SwitchCoverStateEvent : IArchitectureEvent
    {
        public bool IsCover { get; }
        public int CardNumber { get; }

        public SwitchCoverStateEvent(int cardNumber, bool isCover)
        {
            IsCover = isCover;
            CardNumber = cardNumber;
        }
    }
}