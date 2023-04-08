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
        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;

        private int cardNumber;
        private Animator animator;
        private Button button;
        private CardPresenter cardPresenter;

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

        public void SetCardInfo(int cardNumber, CardPresenter presenter)
        {
            this.cardNumber = cardNumber;
            cardPresenter = presenter;

            SetEventRegister();
            SetMatchEffectActive(false);
            RefreshPatternImage();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void SetMatchEffectActive(bool isActive)
        {
            go_matchEffect.SetActive(isActive);
        }

        private void SetEventRegister()
        {
            cardPresenter.RegisterEvent<SwitchCoverStateEvent>(OnSwitchCoverState);
            cardPresenter.RegisterEvent<CardMatchEvent>(OnMatchAndPlayEffect);

            eventRegister.Unregister<FlopCardEvent>(OnFlopCard);
            eventRegister.Register<FlopCardEvent>(OnFlopCard);
        }

        private void RefreshPatternImage()
        {
            img_pattern.sprite = cardPresenter.GetPatternSprite(cardNumber);
        }

        private IEnumerator Cor_PlayDelayMatchEffect()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            SetMatchEffectActive(true);
            // audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_CARD_MATCH);
        }

        private IEnumerator Cor_PlayDelayCoverAnimation()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            // audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_FLOP);
            GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_BACK_SIDE);
        }

        private IEnumerator Cor_FrozeButton(float frozeTimes)
        {
            GetButton.enabled = false;
            yield return new WaitForSeconds(frozeTimes);
            GetButton.enabled = true;
        }

        private void OnSwitchCoverState(SwitchCoverStateEvent eventInfo)
        {
            if (eventInfo.CardNumber != cardNumber)
                return;

            if (eventInfo.IsCover)
                StartCoroutine(Cor_PlayDelayCoverAnimation());
            else
                GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_FRONT_SIDE);
        }

        private void OnMatchAndPlayEffect(CardMatchEvent eventInfo)
        {
            if (eventInfo.CheckIsMatchNumber(cardNumber))
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

        public void OnClickCard()
        {
            cardManager.Flop(cardNumber, out MatchType _);
        }
    }
}