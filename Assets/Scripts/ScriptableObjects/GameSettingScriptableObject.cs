using UnityEngine;

namespace GameCore
{
    public class GameSettingScriptableObject : ScriptableObject
    {
        [SerializeField] private int successIncreasePoint;
        [SerializeField] private int failIncreasePoint;
        [SerializeField] private int pairCount;

        public int GetSuccessIncreasePoint => successIncreasePoint;
        public int GetFailIncreasePoint => failIncreasePoint;
        public int GetPairCount => pairCount;
    }
}