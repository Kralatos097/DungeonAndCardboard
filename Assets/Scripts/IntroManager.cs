using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("Circle Speed")] 
    [SerializeField][Range(0,0.1f)] private float SpeedOnClick;
    [SerializeField][Range(0,0.1f)] private float SpeedUnClick;
    
    [Header("Assign some things")] 
    [SerializeField] private Image CircleSkip;
    [SerializeField] private string SceneToLoad;
    private void FixedUpdate()
    {
        if(CircleSkip.fillAmount == 0) CircleSkip.gameObject.SetActive(false);
        else CircleSkip.gameObject.SetActive(true);
        
        if (Input.GetMouseButton(0)) CircleSkip.fillAmount += SpeedOnClick;
        else CircleSkip.fillAmount -= SpeedUnClick;

        if (CircleSkip.fillAmount == 1) SceneManager.LoadScene(SceneToLoad);
    }

    public void ToTheMainMenu()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
