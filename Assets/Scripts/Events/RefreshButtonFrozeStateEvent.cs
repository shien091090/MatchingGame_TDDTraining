using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class RefreshButtonFrozeStateEvent : IArchitectureEvent
    {
        public bool IsFroze { get; }

        public RefreshButtonFrozeStateEvent(bool isFroze)
        {
            IsFroze = isFroze;
        }
    }
}