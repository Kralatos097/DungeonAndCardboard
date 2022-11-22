using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField][Multiline] private List<String> Dialogue = new List<string>();
    [SerializeField] private List<Sprite> Sprite = new List<Sprite>();

    private int DialogueAvancement;
    
    [Header("Assign some things")]
    [SerializeField] private Animator FadeSprite;
    [SerializeField] private string SceneToLoad;
    [SerializeField] private TextMeshProUGUI TextDisplay;
    [SerializeField] private Image SpriteDisplay;

    public void Start()
    {
        TextDisplay.text = Dialogue[DialogueAvancement];
        SpriteDisplay.sprite = Sprite[DialogueAvancement];
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (DialogueAvancement >= Dialogue.Count-1) SceneManager.LoadScene(SceneToLoad);
            else
            {
                if (Sprite[DialogueAvancement] == Sprite[DialogueAvancement + 1]) NextOption();
                else AnimStart();
            }
        }
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
}
