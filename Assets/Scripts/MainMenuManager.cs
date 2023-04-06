using System;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sessions Name")]
    [Scene][SerializeField] private string Session0;
    [Scene][SerializeField] private string Session1;
    [Scene][SerializeField] private string Credit;
    [Header("Animators")]
    [SerializeField] private Animator TransitionCam;
    [SerializeField] private Animator TableFlip;
    [Header("GameObjects")]
    [SerializeField] private GameObject QuitQuestion;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject WikiDisplay;
    [SerializeField] private GameObject Session0Display;
    [SerializeField] private GameObject Session1Display;

    public bool AsSessionDisplay;
    public bool IsInMenu;
    
    //Check si WEBGL
    private void Awake()
    {
        #if UNITY_WEBGL //a vérifier
            if(QuitButton != null) QuitButton.SetActive(false);
        #endif
    }

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MainMenu");
    }

    private void Update()
    {
        if (!IsInMenu && Input.GetMouseButtonDown(1) || !IsInMenu && Input.GetMouseButtonDown(0))
        {
            IsInMenu = true;
            TransitionCam.SetBool("Menu", true);
        }
        
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

    public void BookAnim()
    {
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

    public void Credits()
    {
        SceneManager.LoadScene(Credit);
    }
    
    //SetActiveFalse tout les sous menus
    public void SetFalseAllMenu()
    {
        QuitQuestion.SetActive(false);
        Settings.SetActive(false);
        WikiDisplay.SetActive(false);
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
        QuitTheGame();
        //TableFlip.SetTrigger("TableFlip");
        //Invoke("QuitTheGame",0.5f);
    }
    public void QuitTheGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
