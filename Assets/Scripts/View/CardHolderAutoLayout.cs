using SNShien.Common.ProcessTools;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class CardHolderAutoLayout : MonoBehaviour
    {
        [Inject] private CardManager cardManager;
        [Inject] private IEventRegister eventRegister;
        private GridLayoutGroup gridLayoutGroup;

        private GridLayoutGroup GetGridLayoutGroup
        {
            get
            {
                if (gridLayoutGroup == null)
                    gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();

                return gridLayoutGroup;
            }
        }

        private void Start()
        {
            eventRegister.Unregister<StartGameEvent>(SetupGridLayout);
            eventRegister.Register<StartGameEvent>(SetupGridLayout);
        }

        private void SetupGridLayout(StartGameEvent eventInfo)
        {
            int constraintCount;
            if (cardManager.GetAllCards.Count <= 7)
                constraintCount = 1;
            else if (cardManager.GetAllCards.Count % 3 == 0)
                constraintCount = 3;
            else
                constraintCount = 2;

            GetGridLayoutGroup.constraintCount = constraintCount;
        }
    }
}