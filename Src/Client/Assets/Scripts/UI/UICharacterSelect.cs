using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Models;
using Services;
using System;

public class UICharacterSelect : MonoBehaviour {

    
    public Image[] Titles;
    public InputField characterName;
    public Text descs;
    public UICharacterView characterView;
    public Text[] className;
    public List<GameObject> uiChars = new List<GameObject>();
    public GameObject uiCharInfo;//角色列表项目预制体
    public Transform uicahrListPos;
    public GameObject selectPanel;
    public GameObject cratePanel;

    private CharacterClass characterClass;
    private  int selectCharacterIdx = -1;

    // Use this for initialization
    void Start ()
    {
        DataManager.Instance.Load();
        CareerNameInit();
        CareerInit();
        InitCharacterSelect(true);
        UserService.Instance.OnCreateCharacter = this.OnCreateCharacter;
        InitPanel(true);

    }

    

    // Update is called once per frame
 //   void Update () {
		
	//}
    /// <summary>
    /// 初始化面板 ture为选择面板 false为创建面板
    /// </summary>
    /// <param name="init"></param>
    private void InitPanel(bool init)
    {
        if (init)
        {
            selectPanel.SetActive(true);
            cratePanel.SetActive(false);
        }
        else
        {
            selectPanel.SetActive(false);
            cratePanel.SetActive(true);
        }
    }
    /// <summary>
    /// 除始化 职业模型、标题、描述
    /// </summary>
    public void CareerInit()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                characterView.characterModels[i].SetActive(true);
                Titles[i].gameObject.SetActive(true);
            }
            else
            {
                characterView.characterModels[i].SetActive(false);
                Titles[i].gameObject.SetActive(false);
            }
        }
        descs.text = DataManager.Instance.Characters[1].Description;
    }
    /// <summary>
    /// 职业名字动态加载
    /// </summary>
    public void CareerNameInit()
    {
        for (int i = 0; i < className.Length; i++)
        {
            className[i].text = DataManager.Instance.Characters[i + 1].Name;
            //Debug.LogFormat("记载{0}", i+1);
        }
    }
    /// <summary>
    /// 创建界面 点击角色后 处理标题和描述
    /// </summary>
    /// <param name="characterClass"></param>
    public void OnSelectClass(int characterClass)
    {
        this.characterClass = (CharacterClass)characterClass + 1;
        characterView.CurrentCharacter = characterClass;
        for (int i = 0; i < Titles.Length; i++)
        {
            if (i == characterClass)
            {
                Titles[i].gameObject.SetActive(true);
            }
            else
            {
                Titles[i].gameObject.SetActive(false);
            }
        }
        descs.text = DataManager.Instance.Characters[characterClass + 1].Description;//加载角色描述

    }
    /// <summary>
    /// 初始化 角色选择菜单中的角色列表
    /// </summary>
    public void InitCharacterSelect(bool init)
    {
        Debug.Log("刷新角色列表");
        //删除上一个用户的角色列表
        if (init)
        {
            foreach (var oldCharUi in uiChars)
            {
                Destroy(oldCharUi);
            }
            uiChars.Clear();
        }
        //更新现在登入用户的角色列表
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(uiCharInfo, uicahrListPos);
            UICharInfo charInfo = go.GetComponent<UICharInfo>();
            charInfo.characterInfo = User.Instance.Info.Player.Characters[i];//更新角色ui的信息
            Button button =go. GetComponent<Button>();
            int idx = i;
            button.onClick.AddListener(() => OnSelectcharacter(idx));
            uiChars.Add(go);
            go.SetActive(true);

        }
    }
    
    /// <summary>
    /// 点击选择角色时 角色模型、当前角色更改，选中标记处理
    /// </summary>
    /// <param name="idx"></param>
    public void OnSelectcharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char :[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrentCharacter = (int)cha.Class-1;
        for (int i = 0; i < uiChars.Count; i++)
        {
            UICharInfo uiCharInfo = uiChars[i]. GetComponent<UICharInfo>();
            uiCharInfo.IsSelect = idx == i;           
        }
    }
   
    /// <summary>
    /// 点击创建角色按钮
    /// </summary>
    public void OnClickCreate()
    {
        //验证角色名是不是空
        //验证完后 发送服务器 参数(角色名字，角色职业)
        if (characterName==null)
        {
            MessageBox.Show("用户名不能为空！");
        }
        else
        {
           
            UserService.Instance.SendCharacterCreate(characterName.text, characterClass);

        }
    }
    private void OnCreateCharacter(Result arg0, string arg1)
    {
        if (arg0==Result.Success)
        {
            MessageBox.Show("创建成功！");
        }
        else
        {
            MessageBox.Show("创建失败！");
        }
        

    }
    public void OnClickStrat()
    {
        //进入地图
        Debug.Log("点击开始游戏");
        UserService.Instance.SenderGameEnter(selectCharacterIdx);
    }
}
