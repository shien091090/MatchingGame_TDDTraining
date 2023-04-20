using System;
using System.Collections;
using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MatchingGameView : MonoBehaviour
    {
        private const string ANIM_KEY_GAME_START = "MainCanvas_GameStart";
        private const string ANIM_KEY_GAME_SETTLE = "MainCanvas_GameSettle";
        private const string PREFAB_KEY = "CardView";

        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;
        [Inject] private CardManager cardManager;
        [SerializeField] private ResetButton resetButton;
        [SerializeField] private float delaySettleTimes;
        [SerializeField] private float delayPlaySettleSoundTimes;
        [SerializeField] private float delayPlaySettleBgmTimes;
        [SerializeField] private ObjectPoolManager cardObjPool;

        private Animator animator;
        private Canvas canvas;

        private Canvas GetCanvas
        {
            get
            {
                if (canvas == null)
                    canvas = GetComponent<Canvas>();

                return canvas;
            }
        }

        private Animator GetAnimator
        {
            get
            {
                if (animator == null)
                    animator = GetComponent<Animator>();

                return animator;
            }
        }

        private void Start()
        {
            InitCamera();
            SetEventRegister();
        }

        private void InitCamera()
        {
            GetCanvas.worldCamera = Camera.main;
        }

        public void PlayIdleAnimation()
        {
            GetAnimator.Play(ANIM_KEY_GAME_START, 0, 0);
        }

        public void HideAllCards()
        {
            cardObjPool.HideAllCards(PREFAB_KEY);
        }

        public void CreateCardPrefab(CardPresenter presenter)
        {
            foreach (Card card in cardManager.GetAllCards)
            {
                CardView cardObj = cardObjPool.PickUpObject<CardView>(PREFAB_KEY);
                if (cardObj == null)
                    continue;

                cardObj.SetCardInfo(card.number, presenter);
                cardObj.Show();
            }
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<FlopCardEvent>(OnFlopCard);
            eventRegister.Register<FlopCardEvent>(OnFlopCard);
        }

        private void PlaySettleAnimation(Action callback)
        {
            StartCoroutine(Cor_PlayGameSettleAnimation(callback));
        }

        private IEnumerator Cor_PlayGameSettleAnimation(Action callback)
        {
            yield return new WaitForSeconds(delaySettleTimes);

            audioManager.SetParam(AudioConstKey.AUDIO_PARAM_FADE_OUT_TIMES, 1);
            audioManager.Stop();

            GetAnimator.Play(ANIM_KEY_GAME_SETTLE, 0, 0);

            yield return new WaitForSeconds(delayPlaySettleSoundTimes);

            audioManager.PlayOneShot(AudioConstKey.AUDIO_KEY_GAME_SETTLE);

            yield return new WaitForSeconds(delayPlaySettleBgmTimes);

            audioManager.SetParam(AudioConstKey.AUDIO_PARAM_FADE_IN_TIMES, 3);
            audioManager.Play(AudioConstKey.AUDIO_KEY_BGM_SETTLE);

            callback?.Invoke();
        }

        private void OnFlopCard(FlopCardEvent eventInfo)
        {
            if (eventInfo.MatchResult != MatchType.MatchAndGameFinish)
                return;

            resetButton.SetButtonEnable(false);
            PlaySettleAnimation(() =>
            {
                resetButton.SetButtonEnable(true);
            });
        }
    }
}