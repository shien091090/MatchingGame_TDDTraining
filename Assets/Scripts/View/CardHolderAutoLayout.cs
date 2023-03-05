using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    public class CardHolderAutoLayout : MonoBehaviour
    {
        [Inject] private CardManager cardManager;
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
            cardManager.OnStartGame -= SetupGridLayout;
            cardManager.OnStartGame += SetupGridLayout;
        }

        private void SetupGridLayout()
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