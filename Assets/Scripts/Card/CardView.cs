using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image img_pattern;
        private Card cardInfo;
        private IPatternSetting patternSetting;

        public void SetCardInfo(Card card, IPatternSetting patternSo)
        {
            cardInfo = card;
            patternSetting = patternSo;

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
    }
}