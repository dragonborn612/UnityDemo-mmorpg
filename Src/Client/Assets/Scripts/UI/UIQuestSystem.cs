using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow {
    public Text title;

    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;

    private bool showAvailableList = false;
	// Use this for initialization
	void Start () {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
        //QuestManager.Instance.OnQuestChanged+=RefreshUI;
	}

    private void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
        if (item.owner==listMain)
        {
            if (listBranch.SelectedItem!=null)
            {
                //listBranch.SelectedItem.GetComponent<Image>().overrideSprite = (listBranch.SelectedItem as UIQuestItem).normalBg;
                listBranch.SelectedItem = null;
            }
        }
        else
        {
            if (listMain.SelectedItem!=null)
            {
                //listMain.SelectedItem.GetComponent<Image>().overrideSprite = (listMain.SelectedItem as UIQuestItem).normalBg;
                listMain.SelectedItem = null;
            }         
        }
    }
    private void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1; //1为可接任务 0为进行中
        RefreshUI();
    }
    private void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanged+=RefreshUI;
    }
    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestList();
    }

    /// <summary>
    /// 初始化任务列表
    /// </summary>
    void InitAllQuestList()
    {
        foreach (var item in QuestManager.Instance.allQuests)
        {
            if (showAvailableList) //显示可接任务
            {
                if (item.Value.Info!=null) //过滤已接任务
                {
                    continue;
                }
            }
            else //显示已接任务
            {
                if (item.Value.Info==null) //过滤未接任务
                {
                    continue;
                }
                if (item.Value.Info.Status == QuestStatus.Finished)
                {
                    continue;
                }
            }
            
            GameObject go = Instantiate(itemPrefab, item.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(item.Value);
            if (item.Value.Define.Type==QuestType.Main)
            {
                this.listMain.AddItem(ui as ListView.ListViewItem);
            }
            else
            {
                this.listBranch.AddItem(ui as ListView.ListViewItem);
            }
        }       
    }
    /// <summary>
    /// 清除销毁所有item
    /// </summary>
    private void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }
   
}
