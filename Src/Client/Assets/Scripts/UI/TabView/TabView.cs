using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour {
    public TabButten[] tabButtens;
    public GameObject[] pages;
    public delegate void TabSelectHandel(int idx);
    public event TabSelectHandel OnTabSelect;
    // Use this for initialization
    //初始化
    IEnumerator Start () {
        for (int i = 0; i < tabButtens.Length; i++)
        {
            tabButtens[i].tabView = this;
            tabButtens[i].tabIndex = i;
        }
        yield return  new WaitForEndOfFrame();
        SelectTab(0);
	}
	
	

    internal void SelectTab(int tabIndex)
    {
        for (int i = 0; i < tabButtens.Length; i++)
        {
            tabButtens[i].Seclect(tabIndex == i); //改按钮图标
            pages[i].SetActive(tabIndex == i); //改页面
        }
        if (OnTabSelect!=null)
        {
            OnTabSelect.Invoke(tabIndex);
        }
    }
}
