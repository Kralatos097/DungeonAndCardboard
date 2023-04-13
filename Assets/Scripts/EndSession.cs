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

    public void ToOutro()
    {
        if (EndGameInfo.SessionNb == 0)
        {
            SceneManager.LoadSceneAsync("OutroS0");
        }
        else
        {
            if (EndGameInfo.IsVictory)
            {
                SceneManager.LoadSceneAsync("OutroS"+EndGameInfo.SessionNb);
            }
            else
            {
                LoadMainMenu();
            }
        }
    }

    private void LoadMainMenu()
    {
        EndGameInfo.IsVictory = false;
        EndGameInfo.IsTuto = false;
        SceneManager.LoadScene(SceneNameToLoad);
    }
    
    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        SetupCharaPanel();
        SetupVictoryState();
    }

    private void SetupCharaPanel()
    {
        SetupWarriorStat(charaPanel.GetChild(0));
        if(!EndGameInfo.IsTuto)
        {
            SetupThiefStat(charaPanel.GetChild(1));
            SetupClericStat(charaPanel.GetChild(2));
            SetupWizardStat(charaPanel.GetChild(3));
        }
        else
        {
            charaPanel.GetChild(1).gameObject.SetActive(false);
            charaPanel.GetChild(2).gameObject.SetActive(false);
            charaPanel.GetChild(3).gameObject.SetActive(false);
        }
    }

    private void SetupWarriorStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(
            WarriorInfo.MaxHp <= 0 || !EndGameInfo.IsVictory);

        if (WarriorInfo.ActiveOne != null)
        {
            chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.ActiveOne);
            chara.GetChild(1).GetComponent<Image>().sprite = WarriorInfo.ActiveOne.logo;
        }

        if (WarriorInfo.ActiveTwo != null)
        {
            chara.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.ActiveTwo);
            chara.GetChild(2).GetComponent<Image>().sprite = WarriorInfo.ActiveTwo.logo;
        }

        if (WarriorInfo.Passive != null)
        {
            chara.GetChild(3).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.Passive);
            chara.GetChild(3).GetComponent<Image>().sprite = WarriorInfo.Passive.logo;
        }

        if (WarriorInfo.Consumable != null)
        {
            chara.GetChild(4).GetComponent<StuffButtonOver>().ChangeStuff(WarriorInfo.Consumable);
            chara.GetChild(4).GetComponent<Image>().sprite = WarriorInfo.Consumable.logo;
        }
    }

    private void SetupThiefStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(
            ThiefInfo.MaxHp <= 0 || !EndGameInfo.IsVictory);

        if (ThiefInfo.ActiveOne != null)
        {
            chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.ActiveOne);
            chara.GetChild(1).GetComponent<Image>().sprite = ThiefInfo.ActiveOne.logo;
        }

        if (ThiefInfo.ActiveTwo != null)
        {
            chara.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.ActiveTwo);
            chara.GetChild(2).GetComponent<Image>().sprite = ThiefInfo.ActiveTwo.logo;
        }

        if (ThiefInfo.Passive != null)
        {
            chara.GetChild(3).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.Passive);
            chara.GetChild(3).GetComponent<Image>().sprite = ThiefInfo.Passive.logo;
        }

        if (ThiefInfo.Consumable != null)
        {
            chara.GetChild(4).GetComponent<StuffButtonOver>().ChangeStuff(ThiefInfo.Consumable);
            chara.GetChild(4).GetComponent<Image>().sprite = ThiefInfo.Consumable.logo;
        }
    }

    private void SetupClericStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(
            ClericInfo.MaxHp <= 0 || !EndGameInfo.IsVictory);


        if (ClericInfo.ActiveOne != null)
        {
            chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.ActiveOne);
            chara.GetChild(1).GetComponent<Image>().sprite = ClericInfo.ActiveOne.logo;
        }

        if (ClericInfo.ActiveTwo != null)
        {
            chara.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.ActiveTwo);
            chara.GetChild(2).GetComponent<Image>().sprite = ClericInfo.ActiveTwo.logo;
        }

        if (ClericInfo.Passive != null)
        {
            chara.GetChild(3).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.Passive);
            chara.GetChild(3).GetComponent<Image>().sprite = ClericInfo.Passive.logo;
        }

        if (ClericInfo.Consumable != null)
        {
            chara.GetChild(4).GetComponent<StuffButtonOver>().ChangeStuff(ClericInfo.Consumable);
            chara.GetChild(4).GetComponent<Image>().sprite = ClericInfo.Consumable.logo;
        }
    }

    private void SetupWizardStat(Transform chara)
    {
        chara.GetChild(0).GetChild(0).gameObject.SetActive(
            WizardInfo.MaxHp <= 0 || !EndGameInfo.IsVictory);

        if (WizardInfo.ActiveOne != null)
        {
            chara.GetChild(1).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.ActiveOne);
            chara.GetChild(1).GetComponent<Image>().sprite = WizardInfo.ActiveOne.logo;
        }

        if (WizardInfo.ActiveTwo != null)
        {
            chara.GetChild(2).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.ActiveTwo);
            chara.GetChild(2).GetComponent<Image>().sprite = WizardInfo.ActiveTwo.logo;
        }

        if (WizardInfo.Passive != null)
        {
            chara.GetChild(3).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.Passive);
            chara.GetChild(3).GetComponent<Image>().sprite = WizardInfo.Passive.logo;
        }

        if (WizardInfo.Consumable != null)
        {
            chara.GetChild(4).GetComponent<StuffButtonOver>().ChangeStuff(WizardInfo.Consumable);
            chara.GetChild(4).GetComponent<Image>().sprite = WizardInfo.Consumable.logo;
        }
    }

    private void SetupVictoryState()
    {
        if(EndGameInfo.IsVictory)
        {
            //todo: victory setup
            audioManager.Play("EndSessionWin");
        }
        else
        {
            //todo: defeat setup
            audioManager.Play("EndSessionLose");
        }
    }
}
