using GameCore;
using TMPro;
using UnityEngine;
using Zenject;

public class PointBoardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp_point;

    [Inject] private PointManager pointManager;

    private void Awake()
    {
        SetEventRegister();
    }

    private void SetEventRegister()
    {
        pointManager.OnPointChange -= OnPointChange;
        pointManager.OnPointChange += OnPointChange;

        pointManager.OnReset -= OnReset;
        pointManager.OnReset += OnReset;
    }

    private void SetPoint(int currentPoint)
    {
        tmp_point.text = currentPoint.ToString();
    }

    private void OnReset()
    {
        SetPoint(0);
    }

    private void OnPointChange(PointChangeEvent eventInfo)
    {
        SetPoint(eventInfo.CurrentPoint);
    }
}