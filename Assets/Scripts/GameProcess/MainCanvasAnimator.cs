using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MainCanvasAnimator : MonoBehaviour
    {
        private const string ANIM_KEY_GAME_START = "MainCanvas_GameStart";
        private const string ANIM_KEY_GAME_SETTLE = "MainCanvas_GameSettle";

        [Inject] private CardManager cardManager;
        [Inject] private IAudioManager audioManager;
        [SerializeField] private float delaySettleTimes;
        [SerializeField] private float delayPlaySettleSoundTimes;
        [SerializeField] private float delayPlaySettleBgmTimes;

        private Animator animator;

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
            SetEventRegister();
        }

        private void SetEventRegister()
        {
            cardManager.OnStartGame -= OnStartGame;
            cardManager.OnStartGame += OnStartGame;

            cardManager.OnFlopCard -= OnFlopCard;
            cardManager.OnFlopCard += OnFlopCard;
        }

        private IEnumerator Cor_PlayGameSettleAnimation()
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
        }

        private void OnFlopCard(MatchType matchResultType)
        {
            if (matchResultType == MatchType.MatchAndGameFinish)
                StartCoroutine(Cor_PlayGameSettleAnimation());
        }

        private void OnStartGame()
        {
            GetAnimator.Play(ANIM_KEY_GAME_START, 0, 0);
        }
    }
}