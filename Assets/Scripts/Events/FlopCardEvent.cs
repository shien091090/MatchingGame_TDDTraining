using SNShien.Common.ArchitectureTools;

namespace GameCore
{
    public class FlopCardEvent : IArchitectureEvent
    {
        public MatchType MatchResult { get; }

        public FlopCardEvent(MatchType matchResult)
        {
            MatchResult = matchResult;
        }
    }
}