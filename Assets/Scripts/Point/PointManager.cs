using System;

namespace GameCore
{
    public class PointManager
    {
        private readonly int failPointDamage;
        private readonly int successIncreasePoint;
        public event Action OnReset;
        public event Action<PointChangeEvent> OnPointChange;

        public int GetPoint { get; private set; }

        public PointManager(int successIncreasePoint = 0, int failPointDamage = 0)
        {
            this.failPointDamage = failPointDamage;
            this.successIncreasePoint = successIncreasePoint;
        }

        public override string ToString()
        {
            return $"{nameof(failPointDamage)}: {failPointDamage}, {nameof(successIncreasePoint)}: {successIncreasePoint}";
        }

        public void AddPoint()
        {
            GetPoint += successIncreasePoint;
            PointChangeEvent pointChangeEvent = new PointChangeEvent(GetPoint, successIncreasePoint);
            OnPointChange?.Invoke(pointChangeEvent);
        }

        public void SubtractPoint()
        {
            GetPoint -= failPointDamage;
            if (GetPoint < 0)
                GetPoint = 0;

            PointChangeEvent pointChangeEvent = new PointChangeEvent(GetPoint, -failPointDamage);
            OnPointChange?.Invoke(pointChangeEvent);
        }

        public void Reset()
        {
            GetPoint = 0;
            OnReset?.Invoke();
        }
    }
}