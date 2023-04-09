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
        [SerializeField] private GameObject go_matchEffect;
        [Inject] private CardManager cardManager;

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
            cardPresenter.RegisterEvent<PlayCardMatchEffectEvent>(OnMatchAndPlayEffect);
            cardPresenter.RegisterEvent<RefreshButtonFrozeStateEvent>(OnSetButtonFroze);
        }

        private void SetButtonEnable(bool isEnable)
        {
            GetButton.enabled = isEnable;
        }

        private void RefreshPatternImage()
        {
            img_pattern.sprite = cardPresenter.GetPatternSprite(cardNumber);
        }

        private IEnumerator Cor_PlayDelayCoverAnimation()
        {
            yield return new WaitForSeconds(delayCoverTimes);

            // audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_FLOP);
            GetAnim.SetTrigger(ANIM_PARAM_FLOP_TO_BACK_SIDE);
        }

        private void OnSetButtonFroze(RefreshButtonFrozeStateEvent eventInfo)
        {
            bool isEnable = eventInfo.IsFroze == false;
            SetButtonEnable(isEnable);
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

        private void OnMatchAndPlayEffect(PlayCardMatchEffectEvent eventInfo)
        {
            if (eventInfo.CheckIsMatchNumber(cardNumber))
                SetMatchEffectActive(true);
        }

        public void OnClickCard()
        {
            cardManager.Flop(cardNumber, out MatchType _);
        }
    }
}