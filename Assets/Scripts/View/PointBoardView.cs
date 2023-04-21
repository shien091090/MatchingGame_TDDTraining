using GameCore;
using SNShien.Common.ProcessTools;
using TMPro;
using UnityEngine;
using Zenject;

public class PointBoardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp_point;

    [Inject] private IEventRegister eventRegister;

    private void Awake()
    {
        SetEventRegister();
    }

    private void SetEventRegister()
    {
        eventRegister.Unregister<PointChangeEvent>(OnPointChange);
        eventRegister.Register<PointChangeEvent>(OnPointChange);

        eventRegister.Unregister<ResetPointEvent>(OnReset);
        eventRegister.Register<ResetPointEvent>(OnReset);
    }

    private void SetPoint(int currentPoint)
    {
        tmp_point.text = currentPoint.ToString();
    }

    private void OnReset(ResetPointEvent eventInfo)
    {
        SetPoint(0);
    }

    private void OnPointChange(PointChangeEvent eventInfo)
    {
        SetPoint(eventInfo.CurrentPoint);
    }
}