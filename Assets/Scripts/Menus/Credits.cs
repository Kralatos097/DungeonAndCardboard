using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Credits : MonoBehaviour
{
    [SerializeField] private Image CircleSkip;
    [SerializeField][Range(0,0.1f)] private float SpeedOnClick;
    [SerializeField][Range(0,0.1f)] private float SpeedUnClick;
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    private void FixedUpdate()
    {
        if(CircleSkip.fillAmount == 0) CircleSkip.gameObject.SetActive(false);
        else CircleSkip.gameObject.SetActive(true);
        
        if (Input.GetMouseButton(1)) CircleSkip.fillAmount += SpeedOnClick;
        else CircleSkip.fillAmount -= SpeedUnClick;

        if (CircleSkip.fillAmount == 1) SceneManager.LoadScene("MainMenu");
    }
}
