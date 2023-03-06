using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class CardView : MonoBehaviour
    {
        private const string ANIM_PARAM_FLOP_TO_FRONT_SIDE = "FlopToFrontSide";
        private const string ANIM_PARAM_FLOP_TO_BACK_SIDE = "FlopToBackSide";

        [SerializeField] private Image img_pattern;
        [SerializeField] private float delayCoverTimes;
        [Inject] private CardManager cardManager;
        [Inject] private PatternSettingScriptableObject patternSetting;

        private Card cardInfo;
        private Animator animator;

        private Animator GetAnim
        {
            get
            {
                if (animator == null)
                    animator = GetComponent<Animator>();

                return animator;
            }
        }

        public void SetCardInfo(Card card)
        {
            cardInfo = card;
            SetPatternImage();
            SetEventRegister();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void SetEventRegister()
        {
            cardInfo.OnSwitchCoverState -= OnSwitchCoverState;
            cardInfo.OnSwitchCoverState += OnSwitchCoverState;
        }

        private void SetPatternImage()
        {
            Sprite patternSprite = patternSetting.GetPatternSprite(cardInfo.GetPattern);
            img_pattern.sprite = patternSprite;
        }

        private IEnumerator Cor_PlayDelayCoverAnimation()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_BACK_SIDE);
        }

        private void OnSwitchCoverState(bool isCardCover)
        {
            if (isCardCover)
                StartCoroutine(Cor_PlayDelayCoverAnimation());
            else
                GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_FRONT_SIDE);
        }

        public void OnClickCard()
        {
            cardManager.Flop(cardInfo.number, out MatchType matchResult);
        }
    }
}