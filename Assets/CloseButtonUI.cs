using UnityEngine;

public class CloseButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Application.Quit");
        Application.Quit();
    }
}
