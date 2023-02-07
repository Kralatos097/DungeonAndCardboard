using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WikiManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Pages = new List<GameObject>();
    [SerializeField] private GameObject LeftButton, RightButton;
    private int SelectedPage;

    public void NextPage()
    {
        //NextPage
        SelectedPage += 1;
        if (SelectedPage == Pages.Count)
        {
            SelectedPage = 0;
        }
        
        //Supprime les boutons au extremité des pages
        if (SelectedPage == 0) LeftButton.SetActive(false);
        else LeftButton.SetActive(true);
        
        if (SelectedPage == Pages.Count - 1) RightButton.SetActive(false);
        else RightButton.SetActive(true);
        
        //Desactive toutes les pages sauf celle demander
        for (int i = 0; i < Pages.Count; i++)
        {
            Pages[i].SetActive(false);
            Pages[SelectedPage].SetActive(true);
        }
    }
    public void PreviousPage()
    {
        //Previous Page
        SelectedPage -= 1;
        if (SelectedPage < 0)
        {
            SelectedPage = Pages.Count - 1;
        }
        
        //Supprime les boutons au extremité des pages
        if (SelectedPage == 0) LeftButton.SetActive(false);
        else LeftButton.SetActive(true);
        
        if (SelectedPage == Pages.Count - 1) RightButton.SetActive(false);
        else RightButton.SetActive(true);
        
        //Desactive toutes les pages sauf celle demander
        for (int i = 0; i < Pages.Count; i++)
        {
            Pages[i].SetActive(false);
            Pages[SelectedPage].SetActive(true);
        }
    }
}
