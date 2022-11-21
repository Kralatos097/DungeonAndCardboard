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
    [SerializeField] private Animator FadeSprite;

    [SerializeField] private TextMeshProUGUI TextDisplay;
    [SerializeField] private Image SpriteDisplay;

    public void Start()
    {
        TextDisplay.text = Dialogue[DialogueAvancement];
        SpriteDisplay.sprite = Sprite[DialogueAvancement];
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) AnimStart();

        if (DialogueAvancement >= Dialogue.Count) SceneManager.LoadScene("Jeu Test");
    }

    public void AnimStart()
    {
        FadeSprite.SetBool("Fade",true);

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
    }
}
