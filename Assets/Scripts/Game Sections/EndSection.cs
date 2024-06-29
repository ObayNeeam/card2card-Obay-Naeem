using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EndSection : GameSectionBase
{
    [SerializeField] private EndSubWidget widget;
    [SerializeField, Range(0f, 5f)] private float waitTime = 5f;
    public override event Action OnSectionEnded;

    public override Tween DisableSection()
    {
        return widget.DeactivateWidget();
    }

    public override Tween EnableSection()
    {
        StartCoroutine(WaittheMessage());
        return widget.ActivateWidget();
    }

    private IEnumerator WaittheMessage()
    {
        float timer = 0;
        while(timer <= waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        OnSectionEnded?.Invoke();
    }

}
