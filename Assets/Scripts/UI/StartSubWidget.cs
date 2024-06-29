using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField,Range(0f,5f)] private float fadeDuration = 2f;
    [SerializeField, Range(1f, 2f)] private float elementsScale = 1.2f;
    public event System.Action OnPlatBtnClick;
    public override Tween ActivateWidget()
    {
        return mainGroup.DOFade(1f, fadeDuration).OnComplete(()=> mainGroup.blocksRaycasts=true);
    }

    public override Tween DeactivateWidget()
    {
        return mainGroup.DOFade(0f, fadeDuration).OnStart(()=> mainGroup.blocksRaycasts = false);
    }
    public void OnPlayBtnClick()
    {
        mainGroup.blocksRaycasts = false;
        OnPlatBtnClick?.Invoke();
    }
}
