using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class GameSection : GameSectionBase
{
    [SerializeField] private GameplaySubWidget gameplayWidget;
    [SerializeField, Range(1f,10f)] private float cardTimer = 5f;
    public override event Action OnSectionEnded;
    public List<int> cardsData;
    public bool[] cardsState;
    public Dictionary<int,UICardBtn> openCards;
    private Dictionary<int, float> openCardsTimer;
    public override Tween DisableSection()
    {
        sectionEnabled = false;
        gameplayWidget.OnUICardClicked -= OnCardPick;
        return gameplayWidget.DeactivateWidget();
    }

    public override Tween EnableSection()
    {
        sectionEnabled = true;
        Tween widget = gameplayWidget.ActivateWidget();
        InitDataStructures();
        gameplayWidget.BuildGrid(cardsData);
        widget.OnComplete(()=>
        {
            gameplayWidget.OnUICardClicked += OnCardPick;
        });
        return widget;
    }
    private void InitDataStructures()
    {
        int totalCards = GameController.Instance.TotalCards;
        cardsData = new List<int>();
        cardsState = new bool[totalCards];
        openCards = new Dictionary<int, UICardBtn>();
        openCardsTimer = new Dictionary<int, float>();
        cardsData = PopulateValues(totalCards);

    }
    /*
     *  0 - 0
     *  1 - 2
     *  
     */
    private List<int> PopulateValues(int totalCards)
    {
        List<int> possiableIndexes = Enumerable.Range(0, totalCards).ToList();
        List<int> possiableValue = Enumerable.Range(0, (totalCards / 2)).ToList();

        int[] cardsValues = new int[totalCards];

        for (int i = 0; i < possiableValue.Count; i++)
        {
            int firstIndex = UnityEngine.Random.Range(0, possiableIndexes.Count);
            cardsValues[possiableIndexes[firstIndex]] = possiableValue[i];
            possiableIndexes.RemoveAt(firstIndex);

            int secondIndex = UnityEngine.Random.Range(0, possiableIndexes.Count);
            cardsValues[possiableIndexes[secondIndex]] = possiableValue[i];
            possiableIndexes.RemoveAt(secondIndex);
        }
        return cardsValues.ToList();
    }
    private void OnCardPick(UICardBtn card)
    {
        card.SetBtnInteractable(false);
        openCards.Add(card.CardIndex, card);
        card.FlipCard(true, 0.25f).OnComplete(() =>
        {
            openCardsTimer.Add(card.CardIndex, cardTimer);
            CheckOpenCards(card);
        });
    }
    private void Update()
    {
        if (!sectionEnabled) return;
        
        if (openCardsTimer.Count <= 0) return;
        List<int> cardsToRemove = new List<int>();
        List<int> cardsKeys = new List<int>(openCardsTimer.Keys);
        foreach(int key in cardsKeys)
        {
            openCardsTimer[key] -= Time.deltaTime;
            
            if (openCardsTimer[key] <= 0) cardsToRemove.Add(key);
        }
        RemoveIdleCards(cardsToRemove);
    }
    private void RemoveIdleCards(List<int> cardsToRemove)
    {
        foreach(int index in cardsToRemove)
        {
            openCardsTimer.Remove(index);
            UICardBtn card = openCards[index];
            openCards.Remove(index);
            card.FlipCard(false,0.25f);
            card.SetBtnInteractable(true);
        }
    }
    private void CheckOpenCards(UICardBtn card)
    {
        if (openCards.Count <= 1) return;

        List<int> keys = new List<int>(openCards.Keys);

        for (int i = 0; i < keys.Count - 1; i++)
        {
            int key = keys[i];

            if (openCards[key].CardType == card.CardType && !cardsState[key])
            {
                Debug.Log("A Match");

                int index1 = card.CardIndex;
                int index2 = key;

                cardsState[index1] = true;
                cardsState[index2] = true;
                
                openCardsTimer.Remove(index1);
                openCardsTimer.Remove(index2);

                openCards[index1].SetBtnInteractable(false);
                openCards[index2].SetBtnInteractable(false);
                
                openCards.Remove(index1);
                openCards.Remove(index2);

                break;
            }
        }
    }
    private void OnGameEnd(bool isWon)
    {
        // deal with game logic
        OnSectionEnded?.Invoke();
    }
}
