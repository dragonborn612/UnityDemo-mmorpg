using Assets.Scripts.Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

    public Text title;

    public Text targets;

    public Text description;
    //public Text overview;
    public UIIconItem[] rewardItems;
    public Sprite noRewardItemSprite;

    public Text rewardMoney;
    public Text rewardExp;

    public void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        //if (this.overview!=null)
        //{
        //    this.overview.text = quest.Define.Overview;
        //}
        if (quest.Info==null)
        {
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            if (quest.Info.Status == QuestStatus.InProgress)
            {
                this.description.text = quest.Define.Dialog;
            }
            if (quest.Info.Status==QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
            if (quest.Info.Status == QuestStatus.Finished)
            {
                this.description.text = "任务已完成！";
            }

        }

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();


        if (quest.Define.RewardItem1 > 0)
        {
            string iconName = DataManager.Instance.Items[quest.Define.RewardItem1].Icon;
            rewardItems[0].SetMainIcon(iconName);
        }
        else
        {
            rewardItems[0].SetMainIcon(noRewardItemSprite);
        }
        if (quest.Define.RewardItem2 > 0)
        {
            string iconName = DataManager.Instance.Items[quest.Define.RewardItem2].Icon;
            rewardItems[1].SetMainIcon(iconName);
        }
        else
        {
            rewardItems[1].SetMainIcon(noRewardItemSprite);
        }
        if (quest.Define.RewardItem3 > 0)
        {
            string iconName = DataManager.Instance.Items[quest.Define.RewardItem3].Icon;
            rewardItems[2].SetMainIcon(iconName);

        }
        else
        {
            rewardItems[2].SetMainIcon(noRewardItemSprite);
        }
    }
        //foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        //{
        //    fitter.SetLayoutVertical();
        //}
        
    
    public void OnClickAbandon()
    {

    }
}
