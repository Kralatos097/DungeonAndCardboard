using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScrollReset : MonoBehaviour
{
    public float scrollResetTime = 2;
    public float scrollResetSpeed = 2;

    private float scrollResetCnt = 0;
    
    private RectTransform tr;

    private void Start()
    {
        tr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tr.anchoredPosition.y > 0)
        {
            scrollResetCnt += Time.deltaTime;

            if(scrollResetCnt >= scrollResetTime)
            {
                tr.position -= new Vector3(0, scrollResetSpeed * Time.deltaTime, 0);
            }
        }
        else
        {
            scrollResetCnt = 0;
        }
    }
}
