using UnityEngine;

namespace GameCore
{
    public class CardView : MonoBehaviour
    {
        private Card cardInfo;

        public void SetCardInfo(Card card)
        {
            cardInfo = card;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}