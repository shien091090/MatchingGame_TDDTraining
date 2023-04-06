using SNShien.Common.ArchitectureTools;

namespace GameCore
{
    public class StartGameEvent : IArchitectureEvent
    {
        public CardPresenter CardPresenter { get; }

        public StartGameEvent(CardPresenter presenter)
        {
            CardPresenter = presenter;
        }
    }
}