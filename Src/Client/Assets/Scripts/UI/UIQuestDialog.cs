using Assets.Scripts.Models;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow {

    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButton;
    public GameObject submitButton;

	public void SetQuest(Quest quest)
    {
        this.quest = quest;
        //更新任务信息
        this.UpdateQuest();
        
        //更新按钮
        if (this.quest.Info==null)//如果是新任务
        {
            openButton.SetActive(true);
            submitButton.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status==QuestStatus.Complated)//如果任务是已完成的
            {
                openButton.SetActive(false);
                submitButton.SetActive(true);
            }
            else//出问题了 两个都隐藏
            {
                openButton.SetActive(false);
                submitButton.SetActive(false);
            }
        }
    }

    private void UpdateQuest()
    {
        if (this.quest!=null)
        {
            if (this.questInfo!=null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }
    
}
