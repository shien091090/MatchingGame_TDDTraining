using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class MainCanvasAnimator : MonoBehaviour
    {
        private const string ANIM_KEY_GAME_START = "MainCanvas_GameStart";
        private const string ANIM_KEY_GAME_SETTLE = "MainCanvas_GameSettle";

        [Inject] private CardManager cardManager;
        [SerializeField] private float delaySettleTimes;

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

            GetAnimator.Play(ANIM_KEY_GAME_SETTLE, 0, 0);
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