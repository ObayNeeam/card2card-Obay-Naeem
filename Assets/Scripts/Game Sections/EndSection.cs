using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndSection : GameSectionBase
{
    [SerializeField] private EndSubWidget widget;
    public override event Action OnSectionEnded;

    public override Tween DisableSection()
    {
        return widget.DeactivateWidget();
    }

    public override Tween EnableSection()
    {
        return widget.ActivateWidget();
    }

}
