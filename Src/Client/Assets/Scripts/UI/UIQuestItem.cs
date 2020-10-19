using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem {
    public Text title;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public Quest quest;
    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title!=null)
        {
            title.text = quest.Define.Name;
        }
    }
}
