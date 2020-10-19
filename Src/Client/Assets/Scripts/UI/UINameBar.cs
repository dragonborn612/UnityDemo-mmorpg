using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {
    public Text avacerName;

    public Camera palyerCamera;

    public Character characeter;

	// Use this for initialization
	void Start () {
        if (characeter!=null)
        {
            UpdataInfo();
        }
	}
	
	// Update is called once per frame
	void Update () {
        //this.transform.forward= palyerCamera.transform.forward;
	}
    void UpdataInfo()
    {
        if (this.name!=null)
        {
            string name = this.characeter.Name + " Lv." + this.characeter.nCharacterInfo.Level;
            if (avacerName.text!=name)
            {
                avacerName.text = name;
            }

        }
    }
}
