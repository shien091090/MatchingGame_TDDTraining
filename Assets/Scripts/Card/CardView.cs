using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image img_pattern;
        [Inject] private CardManager cardManager;
        [Inject] private PatternSettingScriptableObject patternSetting;
        private Card cardInfo;

        public void SetCardInfo(Card card)
        {
            cardInfo = card;
            SetPatternImage();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void SetPatternImage()
        {
            Sprite patternSprite = patternSetting.GetPatternSprite(cardInfo.GetPattern);
            img_pattern.sprite = patternSprite;
        }

        public void OnClickCard()
        {
        }
    }
}