using System;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSession : MonoBehaviour
{
    [SerializeField] private string SceneNameToLoad;

    [Separator]
    [SerializeField] private Transform charaPanel;

    private AudioManager audioManager;
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneNameToLoad);
    }

    private void Start()
    {
        SetupCharaPanel();
        SetupVictoryState();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void SetupCharaPanel()
    {
        SetupWarriorStat(charaPanel.GetChild(0));
        SetupThiefStat(charaPanel.GetChild(1));
        SetupClericStat(charaPanel.GetChild(2));
        SetupWizardStat(charaPanel.GetChild(3));
    }

    private void SetupWarriorStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(WarriorInfo.MaxHp <= 0);

        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.ActiveOne);
        chara.GetChild(1).GetComponent<Image>().sprite = WarriorInfo.ActiveOne.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.ActiveTwo);
        chara.GetChild(2).GetComponent<Image>().sprite = WarriorInfo.ActiveTwo.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.Passive);
        chara.GetChild(3).GetComponent<Image>().sprite = WarriorInfo.Passive.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.Consumable);
        chara.GetChild(4).GetComponent<Image>().sprite = WarriorInfo.Consumable.logo;
    }

    private void SetupThiefStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(ThiefInfo.MaxHp <= 0);

        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.ActiveOne);
        chara.GetChild(1).GetComponent<Image>().sprite = ThiefInfo.ActiveOne.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.ActiveTwo);
        chara.GetChild(2).GetComponent<Image>().sprite = ThiefInfo.ActiveTwo.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.Passive);
        chara.GetChild(3).GetComponent<Image>().sprite = ThiefInfo.Passive.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.Consumable);
        chara.GetChild(4).GetComponent<Image>().sprite = ThiefInfo.Consumable.logo;
    }

    private void SetupClericStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(ClericInfo.MaxHp <= 0);

        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.ActiveOne);
        chara.GetChild(1).GetComponent<Image>().sprite = ClericInfo.ActiveOne.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.ActiveTwo);
        chara.GetChild(2).GetComponent<Image>().sprite = ClericInfo.ActiveTwo.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.Passive);
        chara.GetChild(3).GetComponent<Image>().sprite = ClericInfo.Passive.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.Consumable);
        chara.GetChild(4).GetComponent<Image>().sprite = ClericInfo.Consumable.logo;
    }

    private void SetupWizardStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(WizardInfo.MaxHp <= 0);

        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.ActiveOne);
        chara.GetChild(1).GetComponent<Image>().sprite = WizardInfo.ActiveOne.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.ActiveTwo);
        chara.GetChild(2).GetComponent<Image>().sprite = WizardInfo.ActiveTwo.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.Passive);
        chara.GetChild(3).GetComponent<Image>().sprite = WizardInfo.Passive.logo;
        
        chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.Consumable);
        chara.GetChild(4).GetComponent<Image>().sprite = WizardInfo.Consumable.logo;
    }

    private void SetupVictoryState()
    {
        if(EndGameInfo.IsVictory)
        {
            //todo: victory setup
            audioManager.Play("VictoryFanfare");
        }
        else
        {
            //todo: defeat setup
            audioManager.Play("DefeatFanfare");
        }
    }
}
