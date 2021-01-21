using Assets.Scripts.Services;
using Assets.Scripts.UI.Team;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour {
    public Text teamTitle;
    public UITeamIteam[] Member;
    public ListView list;
	// Use this for initialization
	void Start () {
        if (User.Instance.TeamInfo==null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (var item in Member)
        {
            this.list.AddItem(item);
        }
	}
    private void OnEnable()
    {
        UpdateTeamUI();
    }

    private void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo==null)
        {
            return;
        }
        this.teamTitle.text = string.Format("我的队伍({0}/5)", User.Instance.TeamInfo.Members.Count);

        for (int i = 0; i < 5; i++)
        {
            if (i<User.Instance.TeamInfo.Members.Count)
            {
                this.Member[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                Member[i].gameObject.SetActive(true);
            }
            else
            {
                Member[i].gameObject.SetActive(false);
            }
        }
    }

    internal void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }
    public void OnClickLeave()
    {
        MessageBox.Show("确定离开队伍吗？", "退出队伍", MessageBoxType.Confirm, "确定离开", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest(User.Instance.TeamInfo.Id);
        };
    }
}
