using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueManager : MonoBehaviour
{
    [SerializeField][Multiline] private List<String> Dialogue = new List<string>();
    [SerializeField] private List<Sprite> Sprite = new List<Sprite>();

    private int DialogueAvancement;
    
    [Header("Circle Speed")] 
    [SerializeField][Range(0,0.1f)] private float SpeedOnClick;
    [SerializeField][Range(0,0.1f)] private float SpeedUnClick;
    
    [Header("Assign some things")]
    [SerializeField] private Animator FadeSprite;
    [SerializeField] private TextMeshProUGUI TextDisplay;
    [SerializeField] private Image SpriteDisplay;
    [SerializeField] private Image CircleSkip;
    
    [Space]
    [SerializeField][Scene] private List<string> SceneToLoad;

    private bool pass = false;

    public void Start()
    {
        TextDisplay.text = Dialogue[DialogueAvancement];
        SpriteDisplay.sprite = Sprite[DialogueAvancement];
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (DialogueAvancement >= Dialogue.Count-1) LoadRandomDungeonScene();
            else
            {
                if (Sprite[DialogueAvancement] == Sprite[DialogueAvancement + 1]) NextOption();
                else AnimStart();
            }
        }
    }

    private void FixedUpdate()
    {
        if(CircleSkip.fillAmount == 0) CircleSkip.gameObject.SetActive(false);
        else CircleSkip.gameObject.SetActive(true);
        
        if (Input.GetMouseButton(1)) CircleSkip.fillAmount += SpeedOnClick;
        else CircleSkip.fillAmount -= SpeedUnClick;

        if (CircleSkip.fillAmount == 1) LoadRandomDungeonScene();
    }

    public void AnimStart()
    {
        FadeSprite.SetBool("Fade",true);
        //L'animation contient un event qui lance NextOption()
    }
    
    public void NextOption()
    {
        DialogueAvancement += 1;
        TextDisplay.text = Dialogue[DialogueAvancement];
        SpriteDisplay.sprite = Sprite[DialogueAvancement];
    }

    public void CanClickNow()
    {
        FadeSprite.SetBool("Fade",false);
        //Pour ne pas cliquer trop vite
    }

    private void LoadRandomDungeonScene()
    {
        if(!pass) pass = true;
        else return;
        
        int nb = Random.Range(0, SceneToLoad.Count);

        SceneManager.LoadSceneAsync(SceneToLoad[nb]);
    }
}
