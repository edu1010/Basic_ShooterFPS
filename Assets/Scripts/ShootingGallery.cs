using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGallery : MonoBehaviour
{
    public enum StateGame 
    {
        START,
        PLAYING,
        WIN,
        LOSE
    }

    StateGame m_StateGame;

    int m_Points = 0;
    float m_Countdown = 17.2f;
    int m_TargetsLeft;

    public TextMeshPro m_TextScreen;
    public Text m_TextCountdown;
    public Text m_TextPoints;

    Animation m_Animation;

    private void Start()
    {
        m_StateGame = StateGame.START;

        m_Animation = gameObject.GetComponent<Animation>();
    }

    void Update()
    {
        ShowTextScreen();

        if (m_StateGame == StateGame.PLAYING)
        {
            if (m_Countdown > 0.0f)
                m_Countdown -= Time.deltaTime;

            CheckIfGameEnded();
            ShowInfoGameHud();
        }
        else
        {
            HideGame();
        }
    }

    void ShowInfoGameHud()
    {
        m_TextPoints.text = "POINTS:   " + m_Points;
        m_TextCountdown.text = "TIME LEFT:   " + (int)m_Countdown;
    }

    void HideGame()
    {
        m_TextPoints.text = " ";
        m_TextCountdown.text = " ";

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void ShowTextScreen()
    {
        if(m_StateGame == StateGame.START)
        {
            m_TextScreen.text = "Win in the following Shooting Gallery and the doors will open. Are you ready? Shoot me to start the game.";
        }
        else if (m_StateGame == StateGame.PLAYING)
        {
            m_TextScreen.text = "The game has started. Good luck.";
        }
        else if (m_StateGame == StateGame.WIN)
        {
            m_TextScreen.text = "Congratulations. You won. Do you want to play again?";
        }
        else //lose
        {
            m_TextScreen.text = "Sorry. You lose. You will need to try again";
        }
    }

    void CheckIfGameEnded()
    {
        //Game finished. Change state to win or lose
        if (m_Countdown > 0.0f && m_TargetsLeft <= 0)
        {
            m_StateGame = StateGame.WIN;
        }
        else if (m_Countdown <= 0.0f && m_TargetsLeft > 0)
        {
            m_StateGame = StateGame.LOSE;
        }
    }

    public void ResetGallery()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //reset health
            var monster = transform.GetChild(i).gameObject;
            monster.SetActive(true);
            monster.GetComponent<HealthSystem>().ResetHealth();
        }
        //reset values
        m_Points = 0;
        m_Countdown = 17.2f;
        m_TargetsLeft = transform.childCount;

        m_Animation.Rewind();
        m_Animation.Play();

        m_StateGame = StateGame.PLAYING;
    }

    public StateGame GetShootingGalleryStateGame()
    {
        return m_StateGame;
    }

    public void SetShootingGalleryStateGame(StateGame l_StateGame)
    {
        m_StateGame = l_StateGame;
    }

    public void AddPoints()
    {
        m_Points += 10;
    }

    public void OneTargetLess()
    {   
        m_TargetsLeft--;
        Debug.Log("+1 " + m_TargetsLeft);
    }
}
