using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class UIMessageBox : MonoBehaviour {

    public Text title;
    public Text message;
    public Image[] icons;
    public Button buttonNo;
    public Button buttonYes;
    //public Button buttonClose;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public UnityAction OnYes;
    public UnityAction OnNo;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Init(string title,string message,MessageBoxType type=MessageBoxType.Information,string btnOk="",string btnCancel="")
    {
        if (!string.IsNullOrEmpty(title))
        {
            this.title.text = title;
        }
        this.message.text = message;
        icons[0].enabled = (type == MessageBoxType.Information);
        icons[1].enabled = (type == MessageBoxType.Confirm);
        icons[2].enabled = (type == MessageBoxType.Error);
        if (!string.IsNullOrEmpty(btnOk)) buttonYesTitle.text = btnOk;
        if (!string.IsNullOrEmpty(btnCancel)) buttonNoTitle.text = btnCancel;

        buttonYes.onClick.AddListener(OnClickYes);
        buttonNo.onClick.AddListener(OnClickNo);

        this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);
    }

    private void OnClickYes()
    {
        Destroy(this.gameObject);
        if (OnYes != null)
        {
            OnYes.Invoke();
        }

    }

    private void OnClickNo()
    {
        Destroy(this.gameObject);
        if (OnNo!=null)
        {
            OnNo.Invoke();
        }
    }
}
