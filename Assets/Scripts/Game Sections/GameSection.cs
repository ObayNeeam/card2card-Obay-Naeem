using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameSection : GameSectionBase
{
    [SerializeField] private GameplaySubWidget gameplayWidget;
    [SerializeField, Range(1f,10f)] private float cardTimer = 5f;
    public override event Action OnSectionEnded;
    public List<int> cardsData;
    public bool[] cardsState;
    public Dictionary<int,UICardBtn> openCards;
    private Dictionary<int, float> openCardsTimer;
    private GameStateStructure gameState;
    private int playerMatches;
    private int playerClicks;
    public override Tween DisableSection()
    {
        sectionEnabled = false;
        gameplayWidget.OnUICardClicked -= OnCardPick;
        gameplayWidget.OnHomeBtnPressed -= OnHomeBtnClicked;
        return gameplayWidget.DeactivateWidget();
    }

    public override Tween EnableSection()
    {
        sectionEnabled = true;
        Tween widget = gameplayWidget.ActivateWidget();
        InitDataStructures();
        gameplayWidget.BuildGrid(cardsData, cardsState);
        widget.OnComplete(()=>
        {
            gameplayWidget.OnUICardClicked += OnCardPick;
            gameplayWidget.OnHomeBtnPressed += OnHomeBtnClicked;
        });
        return widget;
    }
    private void OnHomeBtnClicked()
    {
        GameStateHelper.Instance.SaveState(gameState);
        SceneManager.LoadScene(0);
    }
    private void OnApplicationQuit()
    {
        if (!sectionEnabled) return;
        GameStateHelper.Instance.SaveState(gameState);
    }
    private void InitDataStructures()
    {
        int totalCards = GameController.Instance.TotalCards;

        gameState = new GameStateStructure();
        
        if (GameController.Instance.StateLoaded)
        {
            bool result = GameStateHelper.Instance.LoadState(out gameState);
            if (result)
            {
                playerMatches = gameState.userMatches;
                playerClicks = gameState.userClicks;
                cardsState = gameState.cellsState;
                cardsData = gameState.cellsType.ToList();
                GameController.Instance.CardsLayput = gameState.layout;
            }
            else
            {
                InitNewState(totalCards);
            }
        }
        else
        {
            InitNewState(totalCards);
        }

        openCards = new Dictionary<int, UICardBtn>();
        openCardsTimer = new Dictionary<int, float>();
    }
    private void InitNewState(int totalCards)
    {
        gameState.layout = GameController.Instance.CardsLayput;
        cardsData = new List<int>();
        cardsState = new bool[totalCards];
        cardsData = PopulateValues(totalCards);
        gameState.cellsState = cardsState.ToArray();
        gameState.cellsType = cardsData.ToArray();
    }
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
        playerClicks++;
        gameState.userClicks++;
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
                gameState.cellsState = cardsState;
                playerMatches++;
                gameState.userMatches++;
                openCardsTimer.Remove(index1);
                openCardsTimer.Remove(index2);

                openCards[index1].SetBtnInteractable(false);
                openCards[index2].SetBtnInteractable(false);
                
                openCards.Remove(index1);
                openCards.Remove(index2);
                CheckGameEnding();
                break;
            }
        }
    }
    private void CheckGameEnding()
    {
        foreach(bool cellState in cardsState)
        {
            if (!cellState) return;
        }
        OnGameEnd();
    }
    private void OnGameEnd()
    {
        GameStateHelper.Instance.DeleteState();
        // deal with game logic
        OnSectionEnded?.Invoke();
    }
}
