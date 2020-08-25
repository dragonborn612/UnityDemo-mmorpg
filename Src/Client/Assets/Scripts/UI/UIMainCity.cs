using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;

public class UIMainCity : MonoBehaviour {
    public Text characterName;
    public Text characterLevels;

	// Use this for initialization
	void Start () {
        this.AvaterUpdata();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void AvaterUpdata()
    {
        characterLevels.text = User.Instance.CurrentCharacter.Level.ToString();
        characterName.text = string.Format("{0}[{1}]",User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
    }
}
