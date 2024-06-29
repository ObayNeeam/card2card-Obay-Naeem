using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EndSubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private Image backgeoundElements;
    [SerializeField] private Image backgroundFooter;
    [SerializeField] private Image gameLogo;
    [SerializeField] private Image winMsg;
    [SerializeField] private Image loseMsg;
    [SerializeField, Range(0f, 5f)] private float fadeDuration = 1f;
    

    public override Tween ActivateWidget()
    {
        //if(GameController.Instance.UserWon)
        //{
        //    winMsg.gameObject.SetActive(true);
        //    loseMsg.gameObject.SetActive(false);
        //}
        //else
        //{
        //    winMsg.gameObject.SetActive(false);
        //    loseMsg.gameObject.SetActive(true);
        //}
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(backgeoundElements.DOFade(1f, fadeDuration));
        seq.Join(backgroundFooter.DOFade(1f, fadeDuration));
        seq.Join(mainGroup.DOFade(1f, fadeDuration));
        seq.Join(gameLogo.DOFade(1f, fadeDuration));
        return seq;
    }

    public override Tween DeactivateWidget()
    {
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(backgeoundElements.DOFade(0f, fadeDuration));
        seq.Join(mainGroup.DOFade(0f, fadeDuration));
        seq.Join(backgroundFooter.DOFade(0f, fadeDuration));
        seq.Join(gameLogo.DOFade(0f, fadeDuration));
        return seq;
    }
}
