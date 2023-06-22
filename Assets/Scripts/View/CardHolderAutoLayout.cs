using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class CardHolderAutoLayout : MonoBehaviour
    {
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

        public void SetupGridLayout(int allCardCount)
        {
            int constraintCount;
            if (allCardCount <= 7)
                constraintCount = 1;
            else if (allCardCount % 3 == 0)
                constraintCount = 3;
            else
                constraintCount = 2;

            GetGridLayoutGroup.constraintCount = constraintCount;
        }
    }
}