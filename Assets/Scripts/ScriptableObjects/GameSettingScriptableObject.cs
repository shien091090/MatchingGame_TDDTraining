using UnityEngine;

namespace GameCore
{
    public class GameSettingScriptableObject : ScriptableObject, IGameSetting
    {
        [SerializeField] private int successIncreasePoint;
        [SerializeField] private int failIncreasePoint;
        [SerializeField] private int pairCount;
        [SerializeField] private float cardDelayCoverTimes;
        [SerializeField] private float normalFrozeTimes;
        [SerializeField] private float notMatchFrozeTimes;

        public int GetSuccessIncreasePoint => successIncreasePoint;
        public int GetFailIncreasePoint => failIncreasePoint;
        public int GetPairCount => pairCount;
        public float GetCardDelayCoverTimes => cardDelayCoverTimes;

        public float NormalFrozeTimes => normalFrozeTimes;

        public float NotMatchFrozeTimes => notMatchFrozeTimes;
    }
}