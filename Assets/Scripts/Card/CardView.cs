using System.Collections;
using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;
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
        [SerializeField] private GameObject go_matchEffect;
        [Inject] private CardManager cardManager;
        [Inject] private PatternSettingScriptableObject patternSetting;
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;

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
            go_matchEffect.SetActive(false);

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

            cardInfo.OnMatch -= OnMatchAndPlayEffect;
            cardInfo.OnMatch += OnMatchAndPlayEffect;

            eventRegister.Unregister<FlopCardEvent>(OnFlopCard);
            eventRegister.Register<FlopCardEvent>(OnFlopCard);
        }

        private void SetPatternImage()
        {
            Sprite patternSprite = patternSetting.GetPatternSprite(cardInfo.GetPattern);
            img_pattern.sprite = patternSprite;
        }

        private IEnumerator Cor_PlayDelayMatchEffect()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            go_matchEffect.SetActive(true);
            audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_CARD_MATCH);
        }

        private IEnumerator Cor_PlayDelayCoverAnimation()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_FLOP);
            GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_BACK_SIDE);
        }

        private IEnumerator Cor_FrozeButton(float frozeTimes)
        {
            GetButton.enabled = false;
            yield return new WaitForSeconds(frozeTimes);
            GetButton.enabled = true;
        }

        private void OnMatchAndPlayEffect()
        {
            StartCoroutine(Cor_PlayDelayMatchEffect());
        }

        private void OnFlopCard(FlopCardEvent eventInfo)
        {
            switch (eventInfo.MatchResult)
            {
                case MatchType.Match:
                case MatchType.None:
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
            {
                audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_FLOP);
                GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_FRONT_SIDE);
            }
        }

        public void OnClickCard()
        {
            cardManager.Flop(cardInfo.number, out MatchType _);
        }
    }
}