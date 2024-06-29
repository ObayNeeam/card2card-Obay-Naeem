using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] GameSectionBase startSection;
    [SerializeField] GameSectionBase gameSection;
    [SerializeField] GameSectionBase endSection;
    [SerializeField] GameStates gameStartingSection;
    public static GameController Instance { get; private set; }
    public int TotalCards => totalCards;

    private GameStates currentGameState;
    private GameStates prevGameState;
    private GameSectionBase currentSection;
    private GameSectionBase prevSection;
    private int totalCards;

    // Start is called before the first frame update
    private void Awake()
    {
        totalCards = 6;
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        SetStartingSection(gameStartingSection);
        HandleSections();
    }
    private void SetStartingSection(GameStates startingSectionType)
    {
        currentGameState = startingSectionType;
        switch (currentGameState)
        {
            case GameStates.Start:
                {
                    currentSection = startSection;
                    return;
                }
            case GameStates.Game:
                {
                    currentSection = gameSection;
                    return;
                }
            case GameStates.End:
                {
                    currentSection = endSection;
                    return;
                }
        }

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    private void GetNewGameState()
    {
        prevGameState = currentGameState;
        prevSection = currentSection;
        switch (currentGameState)
        {
            case GameStates.Start:
                {
                    currentGameState = GameStates.Game;
                    currentSection = gameSection;
                    return;
                }
            case GameStates.Game:
                {
                    currentGameState = GameStates.End;
                    currentSection = endSection;
                    return;
                }
            case GameStates.End:
                {
                        currentSection = null;
                    prevSection = null;
                    return;
                }
        }
    }
    private void HandleSections()
    {
        DG.Tweening.Sequence seq = DOTween.Sequence();
        if (prevSection != null) seq.Append(prevSection.DisableSection());
        if (currentSection != null)
        {
            seq.Append(currentSection.EnableSection());
            seq.OnComplete(() =>
            {
                currentSection.OnSectionEnded += ChangeGameState;
            });
        }
    }
    private void ChangeGameState()
    {
        currentSection.OnSectionEnded -= ChangeGameState;
        GetNewGameState();
        HandleSections();
    }
}