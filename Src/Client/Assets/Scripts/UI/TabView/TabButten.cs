using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButten : MonoBehaviour {
    public Sprite activeImage;
    private Sprite normalIamge;
    public int tabIndex;
    public bool selected = false;
    public Image tabImage;
    public TabView tabView;
	// Use this for initialization
	void Start () {
        tabImage = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClickTabButten);

	}
    public void Seclect(bool selsect)
    {
        tabImage.overrideSprite = selsect ? activeImage : normalIamge;
    }
    public void OnClickTabButten()
    {
        tabView.SelectTab(this.tabIndex);
    }
}
