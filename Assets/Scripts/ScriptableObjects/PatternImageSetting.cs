using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class PatternImageSetting
    {
        [SerializeField] private int patternNumber;
        [SerializeField] private Sprite patternSprite;

        public int GetPatternNumber => patternNumber;
        public Sprite GetPatternSprite => patternSprite;
    }
}