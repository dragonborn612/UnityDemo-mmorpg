using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour {

    public SkillBridge.Message.NCharacterInfo characterInfo;

    public Text charClass;
    public Text charName;
    public GameObject hightLightImg;
    public bool IsSelect
    {   get
        {
            return hightLightImg.activeSelf;
        }
        set
        {
            hightLightImg.SetActive(value);
        }
    }
                                       // Use this for initialization
    void Start () {
        if (characterInfo != null)
        {
            charClass.text = characterInfo.Class.ToString();
            charName.text = characterInfo.Name.ToString();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
