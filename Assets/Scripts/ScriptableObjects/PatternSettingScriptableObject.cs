using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public interface IPatternSetting
    {
        Sprite GetPatternSprite(int patternNumber);
    }

    public class PatternSettingScriptableObject : ScriptableObject, IPatternSetting
    {
        [SerializeField] private List<PatternImageSetting> patternImageSettings;

        public Sprite GetPatternSprite(int patternNumber)
        {
            PatternImageSetting setting = patternImageSettings.FirstOrDefault(x => x.GetPatternNumber == patternNumber);
            return setting?.GetPatternSprite;
        }
    }
}