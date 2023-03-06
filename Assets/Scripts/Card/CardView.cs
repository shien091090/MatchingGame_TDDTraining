using System;
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
        [SerializeField] private float normalFrozeTimes;
        [SerializeField] private float notMatchFrozeTimes;
        [Inject] private CardManager cardManager;
        [Inject] private PatternSettingScriptableObject patternSetting;

        private Card cardInfo;
        private Animator animator;
        private Button button;

        private Button GetButton
        {
            get
            {
                if (button == null)
                    button = GetComponent<Button>();

                return button;
            }
        }

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

            cardManager.OnFlopCard -= OnFrozeButton;
            cardManager.OnFlopCard += OnFrozeButton;
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

        private IEnumerator Cor_FrozeButton(float frozeTimes)
        {
            GetButton.enabled = false;
            yield return new WaitForSeconds(frozeTimes);
            GetButton.enabled = true;
        }

        private void OnFrozeButton(MatchType matchResult)
        {
            switch (matchResult)
            {
                case MatchType.None:
                case MatchType.Match:
                case MatchType.MatchAndGameFinish:
                case MatchType.WaitForNextCard:
                    StartCoroutine(Cor_FrozeButton(normalFrozeTimes));
                    break;

                case MatchType.NotMatch:
                    StartCoroutine(Cor_FrozeButton(notMatchFrozeTimes));
                    break;
            }
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
            cardManager.Flop(cardInfo.number, out MatchType _);
        }
    }
}