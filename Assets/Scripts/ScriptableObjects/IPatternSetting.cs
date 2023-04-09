using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public interface IPatternSetting
    {
        Sprite GetPatternSprite(int patternNumber);
        List<int> GetPatternNumberList();
    }
}