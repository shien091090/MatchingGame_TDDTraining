using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class PatternSettingScriptableObject : ScriptableObject, IPatternSetting
    {
        [SerializeField] private List<PatternImageSetting> patternImageSettings;

        public Sprite GetPatternSprite(int patternNumber)
        {
            PatternImageSetting setting = patternImageSettings.FirstOrDefault(x => x.GetPatternNumber == patternNumber);
            return setting?.GetPatternSprite;
        }

        public List<int> GetPatternNumberList()
        {
            return patternImageSettings.Select(x => x.GetPatternNumber).ToList();
        }
    }
}