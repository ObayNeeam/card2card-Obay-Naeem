using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UICardBtn : MonoBehaviour,IPointerDownHandler
{
    public RectTransform CardRect => (RectTransform)transform;
    public int CardType => cardType;
    public int CardIndex => cardIndex;
    public event System.Action<UICardBtn> OnCardClick;

    private int cardType;
    private int cardIndex;
    private Color flipColor;
    private Button cardBtn;
    private void OnValidate()
    {
        if (cardBtn == null) cardBtn = GetComponent<Button>();
    }
    private void Start()
    {
        if (cardBtn == null) cardBtn = GetComponent<Button>();
    }
    public void SetCardData(int index, int type, Color flipColor)
    {
        cardType = type;
        cardIndex = index;
        this.flipColor = flipColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnCardClick?.Invoke(this);
    }
    public Tween FlipCard(bool reveal, float tweenTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(CardRect.DOScaleX(0, tweenTime / 2f));
        seq.AppendCallback(() =>
        {
            cardBtn.image.color = reveal ? flipColor : Color.white;
        });
        seq.Append(CardRect.DOScaleX(1, tweenTime/2f));
        return seq;
    }
    public void SetBtnInteractable(bool state)
    {
        cardBtn.interactable = state;
    }
}
