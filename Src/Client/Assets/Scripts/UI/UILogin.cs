using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour {
    public InputField username;
    public InputField passward;
    public Button loginBtn;
    public Toggle rememberUser;
    public Toggle readedTreaty;
    // Use this for initialization
    void Start ()
    {
        loginBtn.onClick.AddListener(OnClickLogin);
        UserService.Instance.OnLogin = this.OnLogin;
        if (1== PlayerPrefs.GetInt("记住账号"))
        {
            username.text = PlayerPrefs.GetString("账号");
        }
	}

    private void OnLogin(Result result, string messge)
    {
        MessageBox.Show(string.Format("登入{0} 原因:{1}", result, messge));
    }

    // Update is called once per frame
    void Update () {
		
	}
    void OnClickLogin()
    {
        if (rememberUser.isOn)
        {
            PlayerPrefs.SetString("账号", username.text.ToString());
            PlayerPrefs.SetInt("记住账号", 1);//1记住账号 2不记住
        }
        else
        {
            PlayerPrefs.SetInt("记住账号",0);
        }
        if (readedTreaty.isOn==false)
        {
            MessageBox.Show("请阅读条款");
            return;
        }
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(passward.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        UserService.Instance.SendLogin(username.text, passward.text);
    }
}
