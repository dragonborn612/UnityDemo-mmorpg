using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using Assets.Scripts.UI;
using System;
using Assets.Scripts.Managers;

public class UIMain : MonoSingleton<UIMain> {
    public Text characterName;
    public Text characterLevels;

    public UITeam TeamWindow;

	// Use this for initialization
 	protected override void OnStart () {
        this.AvaterUpdata();
	}
		
    private void AvaterUpdata()
    {
        characterLevels.text = User.Instance.CurrentCharacter.Level.ToString();
        characterName.text = string.Format("{0}[{1}]",User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
    }
    public void BackCharacterPlane()
    {
        SceneManager.Instance.LoadScene("Levels/CharacterSelect");
        UserService.Instance.SenderGameLeave();
    }

    public void OnClikeUITest()
    {
        UITest uiTest= UIManager.Instance.Show<UITest>();
        uiTest.Onclose += OnUITestClose;
        uiTest.UpdataTitle("这是是一个测试标题");
    }

    private void OnUITestClose(UIWindow sender, WindowResult windowResult)
    {
        MessageBox.Show(string.Format("你点击了{0}", windowResult.ToString()), "确定");
    }

   public void OnClikeBagButten()
    {
        BagManager.Instance.ShowBag();
    }
    public void OnClikeCharacterEquip()
    {
        EquipMananger.Instance.ShowUiEquip();
    }
    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }
    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }
    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }
    public void OnClickGuild()
    {
        GUildManager.Instance.ShowGuild();
    }
}
