using System;
using SNShien.Common.ArchitectureTools;
using Zenject;

namespace GameCore
{
    public class PointManager
    {
        private IEventInvoker eventInvoker;
        private readonly int failPointDamage;
        private readonly int successIncreasePoint;

        public int GetPoint { get; private set; }

        public PointManager(int successIncreasePoint = 0, int failPointDamage = 0)
        {
            this.failPointDamage = failPointDamage;
            this.successIncreasePoint = successIncreasePoint;
        }

        [Inject]
        public void Construct(IEventInvoker _eventInvoker)
        {
            eventInvoker = _eventInvoker;
        }

        public void AddPoint()
        {
            GetPoint += successIncreasePoint;
            eventInvoker.SendEvent(new PointChangeEvent(GetPoint, successIncreasePoint));
        }

        public void SubtractPoint()
        {
            GetPoint -= failPointDamage;
            if (GetPoint < 0)
                GetPoint = 0;

            eventInvoker.SendEvent(new PointChangeEvent(GetPoint, -failPointDamage));
        }

        public void Reset()
        {
            GetPoint = 0;
            eventInvoker.SendEvent(new ResetPointEvent());
        }
    }
}