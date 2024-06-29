using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySubWidget : SubWidgetControllerBase
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private UICardBtn cardPrefab;
    [SerializeField] private List<Color> cardsColorTypes;
    public event System.Action<UICardBtn> OnUICardClicked;
    List<UICardBtn> cards;
    [SerializeField,Range(0f,5f)] private float fadeDuration = 2f;
#if UNITY_EDITOR
    public void BuildGridEditor(List<int> cardsData)
    {
        //BuildGrid(10);
    }
#endif
    public override Tween ActivateWidget()
    {
#if UNITY_EDITOR
        //BuildGridEditor();
#endif
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(mainGroup.DOFade(1f, fadeDuration));
        return seq;
    }
    public void BuildGrid(List<int> cardsData)
    {
        cards = new List<UICardBtn>();
        for (int i = 0; i < cardsData.Count; i++)
        {
            UICardBtn card =  Instantiate(cardPrefab, cardParent);
            int cardType = cardsData[i];

            card.SetCardData(i, cardType, cardsColorTypes[cardType]);

            cards.Add(card);
            card.OnCardClick += OnCardClicked;
        }
    }
    private void OnCardClicked(UICardBtn card)
    {
        OnUICardClicked?.Invoke(card);
    }
    public override Tween DeactivateWidget()
    {

        foreach (UICardBtn card in cards) card.OnCardClick -= OnCardClicked;
        cards.Clear();

        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(mainGroup.DOFade(0f, fadeDuration));
        return seq;
    }
}
