using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Dialogue
{
    public Sprite LeftSprite;
    public Sprite RightSprite;
    [Multiline] public string Text;
    public bool LeftIsTalking;
    public bool RightIsTalking;
}
public class NewDialogueManager : MonoBehaviour
{
    [SerializeField] private List<Dialogue> dialogue = new List<Dialogue>();
    
    [Header("Assign some things")]
    [SerializeField] private TextMeshProUGUI TextDisplay;
    [SerializeField] private Image LeftSpriteDisplay;
    [SerializeField] private Image RightSpriteDisplay;
    [SerializeField] private Image CircleSkip;

    [Header("Circle Speed")] 
    [SerializeField][Range(0,0.1f)] private float SpeedOnClick;
    [SerializeField][Range(0,0.1f)] private float SpeedUnClick;
    private int DialogueAvancement;
    
    private bool pass = false;
    private Color Grey = new Color(0.5f,0.5f,0.5f,0.5f);

    [Space]
    [SerializeField][Scene] private List<string> SceneToLoad;

    private void Start()
    {
        CheckAll();
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (DialogueAvancement >= dialogue.Count-1) LoadRandomDungeonScene();
            else
            { 
                NextOption();
            }
        }
    }
    
    public void NextOption()
    {
        DialogueAvancement += 1;
        CheckAll();
    }

    private void LeftIsTalking()
    {
        LeftSpriteDisplay.color = Color.white;
        RightSpriteDisplay.color = Grey;
    }

    private void RightIsTalking()
    {
        RightSpriteDisplay.color = Color.white;
        LeftSpriteDisplay.color = Grey;
    }

    private void CheckAll()
    {
        //Check les text et sprites
        TextDisplay.text = dialogue[DialogueAvancement].Text;
        LeftSpriteDisplay.sprite = dialogue[DialogueAvancement].LeftSprite;
        RightSpriteDisplay.sprite = dialogue[DialogueAvancement].RightSprite;
        //Check les bool de qui parle
        if(dialogue[DialogueAvancement].LeftIsTalking) LeftIsTalking();
        if(dialogue[DialogueAvancement].RightIsTalking) RightIsTalking();
    }
    
    private void FixedUpdate()
    {
        if(CircleSkip.fillAmount == 0) CircleSkip.gameObject.SetActive(false);
        else CircleSkip.gameObject.SetActive(true);
        
        if (Input.GetMouseButton(1)) CircleSkip.fillAmount += SpeedOnClick;
        else CircleSkip.fillAmount -= SpeedUnClick;

        if (CircleSkip.fillAmount == 1) LoadRandomDungeonScene();
    }
    
    private void LoadRandomDungeonScene()
    {
        if(!pass) pass = true;
        else return;
        
        int nb = Random.Range(0, SceneToLoad.Count);

        SceneManager.LoadScene(SceneToLoad[nb]);
    }
}
