using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>//单例
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegiester);//如果接到注册请求消息则调用OnRegiester
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCharacterCrate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
        }

       
        public void Init()
        {
            Log.InfoFormat("UserServicce:UuserService已启动");
        }
        private void OnRegiester(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest:User:{0} Password:{1}", request.User, request.Passward);

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user!=null)
            {
                netMessage.Response.userRegister.Result = Result.Failed;
                netMessage.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {
                TPlayer player= DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                netMessage.Response.userRegister.Result = Result.Success;
                netMessage.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(netMessage);
            sender.SendData(data, 0, data.Length);

        }
        private void OnLogin(NetConnection<NetSession> sender, UserLoginRequest loginRequest)
        {
            Log.InfoFormat("UserLoginRequest:User{0} Password:{1}", loginRequest.User, loginRequest.Passward);
            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == loginRequest.User).FirstOrDefault();
            if (user!=null)
            {
                if (loginRequest.Passward==user.Password)
                {
                    sender.Session.User = user;//当前会话的用户

                    netMessage.Response.userLogin.Result = Result.Success;
                    netMessage.Response.userLogin.Errormsg = "None";
                    //在返回消息中添加用户 角色信息
                    netMessage.Response.userLogin.Userinfo = new NUserInfo();
                    netMessage.Response.userLogin.Userinfo.Id = 1;                 
                    netMessage.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    netMessage.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var character in user.Player.Characters)
                    {
                        NCharacterInfo info = new NCharacterInfo();
                        info.Id = character.ID;
                        info.Class = (CharacterClass)character.Class;
                        info.Name = character.Name;
                        netMessage.Response.userLogin.Userinfo.Player.Characters.Add(info);
                    }

                }
                else
                {
                    netMessage.Response.userLogin.Result = Result.Failed;
                    netMessage.Response.userLogin.Errormsg = "密码错误";
                }
            }
            else
            {
                netMessage.Response.userLogin.Result = Result.Failed;
                netMessage.Response.userLogin.Errormsg = "用户名不存在";
            }
            byte[] data = PackageHandler.PackMessage(netMessage); //打包二进制
            sender.SendData(data, 0, data.Length); //发送到客户端
        }
        private void OnCharacterCrate(NetConnection<NetSession> sender, UserCreateCharacterRequest message)
        {
            Log.InfoFormat("UserCharacterCrateRequset:CharacterName:{0} ChaaracterClass:{1}", message.Name, message.Class);

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.createChar = new UserCreateCharacterResponse();

            TCharacter character = new TCharacter()
            {
                Name = message.Name,
                Class = (int)message.Class,
                TID = (int)message.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY=4000,
                MapPosZ=820,
                

            };
            DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            foreach (var cha in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = cha.ID;
                info.Class = (CharacterClass)cha.Class;
                info.Name = cha.Name;
                netMessage.Response.createChar.Characters.Add(info);
            }

            netMessage.Response.createChar.Result = Result.Success;
            netMessage.Response.createChar.Errormsg = "None";

            byte[] date = PackageHandler.PackMessage(netMessage);
            sender.SendData(date, 0, date.Length);



        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest message)
        {

            TCharacter dbCharacter = sender.Session.User.Player.Characters.ElementAt(message.characterIdx);
            Log.InfoFormat("UserGameEnteRequset:CharacterIdx{0} ", message.characterIdx);
            
            Character character= CharacterManager.Instance.AddCharacter(dbCharacter);//角色管理器添加角色

            MapManager.Instance[dbCharacter.MapID].CharacterEnter(sender, character);//调用Map类的角色进入

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.gameEnter = new UserGameEnterResponse();
            netMessage.Response.gameEnter.Result = Result.Success;
            netMessage.Response.gameEnter.Errormsg = "None";

            byte[] data = PackageHandler.PackMessage(netMessage);
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;//在会话中绑定当前角色
        }

    }
}
