using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {
    public InputField userName;
    public InputField password;
    public InputField comfirmPassword;
    public Button register;
    // Use this for initialization
    void Start () {
        register.onClick.AddListener(OnclickRegister);
        UserService.Instance.OnRegister = this.OnRegister;
	}

     void OnRegister(SkillBridge.Message.Result result, string msg)
    {
        MessageBox.Show(string.Format("结果：{0} msg：{1}", result, msg));
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void OnclickRegister()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        else if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        else if (string.IsNullOrEmpty(comfirmPassword.text))
        {
            MessageBox.Show("请确认密码");
            return;
        }
        if (password.text!=comfirmPassword.text)
        {
            MessageBox.Show("两次密码不同");
            return;
        }
        UserService.Instance.SendRegister(this.userName.text, this.password.text);
    }
}
//我的