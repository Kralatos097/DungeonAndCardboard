using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string Sessions0Name, Sessions1Name;

    public void NewGame()
    {
        SceneManager.LoadScene(Sessions0Name);
    }

    public void QuitTheGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
