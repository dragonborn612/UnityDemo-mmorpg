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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

       

        public void Init()
        {
            Log.InfoFormat("UserServicce:UuserService已启动");
        }
        private void OnRegiester(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest:User:{0} Password:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user!=null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {
                TPlayer player= DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }

            sender.SendResponse();

        }
        private void OnLogin(NetConnection<NetSession> sender, UserLoginRequest loginRequest)
        {
            Log.InfoFormat("UserLoginRequest:User{0} Password:{1}", loginRequest.User, loginRequest.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == loginRequest.User).FirstOrDefault();
            if (user!=null)
            {
                if (loginRequest.Passward==user.Password)
                {
                    sender.Session.User = user;//当前会话的用户

                    sender.Session.Response.userLogin.Result = Result.Success;
                    sender.Session.Response.userLogin.Errormsg = "None";
                    //在返回消息中添加用户 角色信息
                    sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                    sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                    sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                    sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                    foreach (var character in user.Player.Characters)
                    {
                        NCharacterInfo info = new NCharacterInfo();
                        info.Id = character.ID;//character.ID;
                        info.Class = (CharacterClass)character.Class;
                        info.Type = CharacterType.Player;
                        info.Name = character.Name;
                        info.Tid = character.ID;
                        sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                    }

                }
                else
                {
                    sender.Session.Response.userLogin.Result = Result.Failed;
                    sender.Session.Response.userLogin.Errormsg = "密码错误";
                }
            }
            else
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户名不存在";
            }
            sender.SendResponse();
        }
        private void OnCharacterCrate(NetConnection<NetSession> sender, UserCreateCharacterRequest message)
        {
            Log.InfoFormat("UserCharacterCrateRequset:CharacterName:{0} ChaaracterClass:{1}", message.Name, message.Class);

            sender.Session.Response.createChar = new UserCreateCharacterResponse();

            TCharacter character = new TCharacter()
            {
                Name = message.Name,
                Class = (int)message.Class,
                TID = (int)message.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 100000,//初始10万金币
                Equips = new byte[28],//int4个字节 7个槽位

            };

            TCharacterBag bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.TCharacterBags.Add(bag);

            character= DBService.Instance.Entities.Characters.Add(character);

            character.Items.Add(new TCharacterItem()
            {
                Owner=character,
                ItemID=1,
                ItemCount=20,
            });
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,
            });

            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            foreach (var cha in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                //info.Id = cha.ID;
                //info.Class = (CharacterClass)cha.Class;
                //info.Name = cha.Name;
                info.Id = 0;//character.ID;
                info.Class = (CharacterClass)cha.Class;
                info.Type = CharacterType.Player;
                info.Name = cha.Name;
                info.Tid = cha.ID;
                sender.Session.Response.createChar.Characters.Add(info);
            }

            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";

            sender.SendResponse();
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest message)
        {
            TCharacter dbCharacter = sender.Session.User.Player.Characters.ElementAt(message.characterIdx);

            Log.InfoFormat("UserGameEnterRequest: characterId:{0}:{1} Map:{2}", dbCharacter.ID, dbCharacter.Name, dbCharacter.MapID);


            Character character= CharacterManager.Instance.AddCharacter(dbCharacter);//角色管理器添加角色

            MapManager.Instance[dbCharacter.MapID].CharacterEnter(sender, character);//调用Map类的角色进入

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";
            sender.Session.Response.gameEnter.Character = character.Info;//道具消息

            /* #region 道具测试用例
             int itemID = 1;
             bool hasItem = character.ItemManager.HasItem(itemID);
             Log.InfoFormat("HasItem[{0}]{1}", itemID, hasItem);
             if (hasItem)
             {
                 //character.ItemManager.RemoveItem(itemID, 1);
             }
             else
             {
                 character.ItemManager.AddItem(1,200);
                 character.ItemManager.AddItem(2, 100);
                 character.ItemManager.AddItem(3, 30);
                 character.ItemManager.AddItem(4, 120);
             }
            // Models.Item item = character.ItemManager.GetItem(itemID);
            // Log.InfoFormat("Item:[{0}][{1}]", itemID, item);
             #endregion*/

            sender.SendResponse();
            sender.Session.Character = character;//在会话中绑定当前角色
        }
        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequset:CharacterId:{0}:{1} Map:{2} ", character.Info.Id, character.Info.Name, character.Data.MapID);
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
            //sender.Session.Character = null;
        }

        public  void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
