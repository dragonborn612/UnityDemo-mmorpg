using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        //注册客户端响应服务器反馈结果的事件
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnCreateCharacter;
        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()//创建时执行
        {
            Debug.Log("UserService()");
            NetClient.Instance.OnConnect += OnGameServerConnect;//连上事件
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;//断开事件
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);//反馈响应
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);

        }

        

        public void Dispose()//销毁时执行
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()//连接服务器
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)//判断服务器连接
            {
                this.connected = true;
                if (this.pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);//补发
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result, string reason)//断链通知
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister != null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                if (this.pendingMessage.Request.userLogin != null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }

                return true;
            }
            return false;
        }

        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequsest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);

            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        public void SendCharacterCreate(string characterName, CharacterClass characterClass)
        {
            Debug.LogFormat("UserCharacterCreateRequsest::characterName :{0} characterClass:{1}", characterName, characterClass.ToString());
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = characterName;
            message.Request.createChar.Class = characterClass;
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);

            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        public void SenderGameEnter(int characterIdx)
        {
            Debug.LogFormat("UserGameEnterRequsest::characterindex :{0} ", characterIdx);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIdx;
            NetClient.Instance.SendMessage(message);
        }
        public void SenderGameLeave()
        {
            Debug.Log("UserGameLeaveReausest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }





        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);


            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }

        private void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnUserLogin:{0} [{1}]", response.Result, response.Errormsg);
            // 如果成功 更新玩家角色表信息
            if (response.Result == Result.Success)
            {
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }
        private void OnUserCreateCharacter(object sender, UserCreateCharacterResponse message)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", message.Result, message.Errormsg);
            // 如果成功 更新玩家角色表信息
            if (message.Result == Result.Success)
            {
                Models.User.Instance.SetupUserCharacterInfo(message.Characters);
            }

            if (OnCreateCharacter != null)
            {
                this.OnCreateCharacter.Invoke(message.Result, message.Errormsg);
            }
        }
        private void OnGameEnter(object sender, UserGameEnterResponse message)
        {
            Debug.LogFormat("OnGameEnterCharacter:{0} [{1}]", message.Result, message.Errormsg);
            if (message.Result == Result.Success)
            {

            }
        }
        private void OnGameLeave(object sender, UserGameLeaveResponse message)
        {
            MapService.Instance.CurrentMapId = 0;
            Debug.LogFormat("OnGameLeave:{0} [{1}]", message.Result, message.Errormsg);

        }
       
        //private void OnCharacterEnter(object sender, MapCharacterEnterResponse message)
        //{
        //    Debug.LogFormat("OnCharacterEnter:character {0} mapId {1}", message.Characters[0], message.mapId);
        //    NCharacterInfo nCharacterInfo = message.Characters[0];
        //    User.Instance.CurrentCharacter = nCharacterInfo;
        //    SceneManager.Instance.LoadScene(DataManager.Instance.Maps[message.mapId].Resource);
        //}
    }
}
