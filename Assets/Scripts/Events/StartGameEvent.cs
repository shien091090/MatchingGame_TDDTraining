using SNShien.Common.ProcessTools;

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