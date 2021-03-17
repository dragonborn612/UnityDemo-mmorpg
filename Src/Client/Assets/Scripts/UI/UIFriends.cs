using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class UIFriends:UIWindow
    {
        public GameObject itemPrefab;
        public ListView listMain;
        public Transform itemRoot;
        public UIFrindItem selectedItem;

        private void Start()
        {
            FriendService.Instance.OnFriendUpdate = RefreshUI;
            this.listMain.onItemSelected += this.OnFriendSelected;
            RefreshUI();
        }
        private void OnDestroy()
        {
            FriendService.Instance.OnFriendUpdate -= RefreshUI;
        }


        private void OnFriendSelected(ListView.ListViewItem item)
        {
            this.selectedItem = item as UIFrindItem;
        }

        public void OnClickFriendAdd()
        {
            InputBox.Show("输入要添加的好友或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
        }

        private bool OnFriendAddSubmit(string input, out string tips)
        {
            tips = "";
            int friendId = 0;
            string friendName = "";
            if (!int.TryParse(input,out friendId))
            {
                friendName = input;
            }
            if (friendId==User.Instance.CurrentCharacter.Id||friendName==User.Instance.CurrentCharacter.Name)
            {
                tips = "开玩笑吗？不能添加自己哦";
                return false;
            }
            FriendService.Instance.SendFriendAddRequest(friendId, friendName);
            return true;
        }
        public void OnClickFriendChat()
        {
            MessageBox.Show("暂未开放");
        }

        public void OnClickFriendRemove()
        {
            if (selectedItem==null)
            {
                MessageBox.Show("请选择要删除的好友");
                return;
            }
            MessageBox.Show(string.Format("确定要删除好友{0}吗？", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
            {
                FriendService.Instance.ScendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
            };
        }


        public void OnClickFriendTeamInvite()
        {
            if (selectedItem==null)
            {
                MessageBox.Show("请选择要邀请的好友");
                return;
            }
            if (selectedItem.Info.Status==0)
            {
                MessageBox.Show("您邀请的好友不在线");
                return;
            }
            MessageBox.Show(string.Format("确定要邀请【{0}】加入队伍吗？", selectedItem.Info.friendInfo.Name), "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
           {
               TeamService.Instance.SendTeamInvitRequest(this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
           };

        }
        private void RefreshUI()
        {
            ClearFriendList();
            InitFriendItem();
        }

        void InitFriendItem()
        {
            foreach (var item in FriendManager.Instance.allFriends)
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                go.SetActive(true);
                UIFrindItem ui = go.GetComponent<UIFrindItem>();
                ui.SetFriendInfo(item);
                this.listMain.AddItem(ui);
            }
        }
        void ClearFriendList()
        {
            this.listMain.RemoveAll();
        }
    }
}
