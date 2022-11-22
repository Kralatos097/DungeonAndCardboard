using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    private string DungeonScene = "DungeonScene";
    [SerializeField] private GameObject quitButton;

    private void Start()
    {
        #if UNITY_WEBGL //a vérifier
            if(quitButton != null) quitButton.SetActive(false);
        #endif
    }

    public void LaunchDungeonButton()
    {
        SceneManager.LoadSceneAsync(DungeonScene);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
