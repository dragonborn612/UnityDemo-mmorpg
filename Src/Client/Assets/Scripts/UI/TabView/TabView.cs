using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour {
    public TabButten[] tabButtens;
    public GameObject[] pages;

	// Use this for initialization
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
            tabButtens[i].Seclect(tabIndex == i);
            pages[i].SetActive(tabIndex == i);
        }
    }
}
