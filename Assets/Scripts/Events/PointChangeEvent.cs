using SNShien.Common.ArchitectureTools;

namespace GameCore
{
    public class PointChangeEvent : IArchitectureEvent
    {
        public int CurrentPoint { get; }
        public int IncreasePoint { get; }

        public PointChangeEvent(int currentPoint, int increasePoint)
        {
            CurrentPoint = currentPoint;
            IncreasePoint = increasePoint;
        }
    }
}