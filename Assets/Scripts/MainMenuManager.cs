using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sessions Name")]
    [SerializeField] private string Session0;
    [SerializeField] private string Session1;
    [Header("Animators")]
    [SerializeField] private Animator TransitionCam;
    [SerializeField] private Animator TableFlip;
    [Header("GameObjects")]
    [SerializeField] private GameObject QuitQuestion;
    [SerializeField] private GameObject Session0Display;
    [SerializeField] private GameObject Session1Display;

    public bool AsSessionDisplay;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !TransitionCam.GetBool("InAnimation") && !AsSessionDisplay)
        {
            MainScreen();
            SetFalseAllMenu();
        } 
        else if (Input.GetMouseButtonDown(1))
        {
            AsSessionDisplay = false;
            SetFalseAllMenu();
        }
    }
    
    //Anim event
    public void SetAnimationTrue()
    {
        TransitionCam.SetBool("InAnimation", true);
    }
    public void SetAnimationFalse()
    {
        TransitionCam.SetBool("InAnimation", false);
    }
    
    //Transi sur la sessions screen
    public void SessionScreen()
    {
        if (!TransitionCam.GetBool("InAnimation"))
        {
            TransitionCam.SetBool("Session",true);
        }
    }
        
    //Transi sur le MainMenu
    public void MainScreen()
    {
        TransitionCam.SetBool("Session",false);
    }
    
    //Load la session...
    public void SessionZero()
    {
        SceneManager.LoadScene(Session0);
    }
    public void SessionOne()
    {
        SceneManager.LoadScene(Session1);
    }
    
    //SetActiveFalse tout les sous menus
    public void SetFalseAllMenu()
    {
        QuitQuestion.SetActive(false);
        Session0Display.SetActive(false);
        Session1Display.SetActive(false);
    }
    
    //Check si un display session est ouvert
    public void DisplaySessionTrue()
    {
        AsSessionDisplay = true;
    }

    //Quit le jeu
    public void TableFlipBeforeQuit()
    {
        TableFlip.SetTrigger("TableFlip");
        Invoke("QuitTheGame",0.5f);
    }
    public void QuitTheGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
