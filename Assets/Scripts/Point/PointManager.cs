namespace GameCore
{
    public class PointManager
    {
        private readonly int failPointDamage;
        private readonly int successIncreasePoint;

        public int GetPoint { get; private set; }

        public PointManager(int successIncreasePoint, int failPointDamage)
        {
            this.failPointDamage = failPointDamage;
            this.successIncreasePoint = successIncreasePoint;
        }

        public PointManager()
        {
            failPointDamage = 0;
            successIncreasePoint = 0;
        }

        public void AddPoint()
        {
            GetPoint += successIncreasePoint;
        }

        public void SubtractPoint()
        {
            GetPoint -= failPointDamage;
            if (GetPoint < 0)
                GetPoint = 0;
        }

        public void Reset()
        {
            GetPoint = 0;
        }
    }
}