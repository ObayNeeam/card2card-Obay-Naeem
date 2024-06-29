using DG.Tweening;
using System;
using UnityEngine;

public class StartSection : GameSectionBase
{
    [SerializeField] private StartSubWidget startWidget;
    public override event Action OnSectionEnded;

    public override Tween DisableSection()
    {
        return startWidget.DeactivateWidget();
    }


    public override Tween EnableSection()
    {
        startWidget.OnPlatBtnClick += OnPlatBtnClick;
        Tween widgetTween = startWidget.ActivateWidget();
        return widgetTween;
    }
    private void OnPlatBtnClick()
    {
        OnSectionEnded?.Invoke();
    }
}
