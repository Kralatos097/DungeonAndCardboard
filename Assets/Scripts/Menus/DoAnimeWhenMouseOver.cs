using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoAnimeWhenMouseOver : MonoBehaviour
{
    public Animator animator;
    private void OnMouseEnter()
    {
        animator.SetTrigger("Go");
    }
}
