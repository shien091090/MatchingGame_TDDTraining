using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class ResetButton : MonoBehaviour
    {
        [Inject] private CardManager cardManager;
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

        private void Start()
        {
            GetButton.onClick.RemoveAllListeners();
            GetButton.onClick.AddListener(() =>
            {
                cardManager.RestartGame();
            });
        }

        public void SetButtonEnable(bool isEnable)
        {
            GetButton.enabled = isEnable;
        }
    }
}