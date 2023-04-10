using System;
using System.Collections;
using SNShien.Common.ArchitectureTools;
using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MainCanvasAnimator : MonoBehaviour
    {
        private const string ANIM_KEY_GAME_START = "MainCanvas_GameStart";
        private const string ANIM_KEY_GAME_SETTLE = "MainCanvas_GameSettle";

        [Inject] private IAudioManager audioManager;
        [Inject] private IEventRegister eventRegister;
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

        public void PlaySettleAnimation(Action callback)
        {
            StartCoroutine(Cor_PlayGameSettleAnimation(callback));
        }

        public void PlayIdleAnimation()
        {
            GetAnimator.Play(ANIM_KEY_GAME_START, 0, 0);
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
    }
}