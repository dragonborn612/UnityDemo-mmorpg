using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>//单例
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegiester);//如果接到注册请求消息则调用OnRegiester
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
        }

       
        public void Init()
        {
            Log.InfoFormat("UserServicce:UuserService已启动");
        }
        private void OnRegiester(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest:User{0} Password{1}", request.User, request.Passward);

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
            Log.InfoFormat("UserLoginRequest:User{0} Password{1}", loginRequest.User, loginRequest.Passward);
            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == loginRequest.User).FirstOrDefault();
            if (user!=null)
            {
                if (loginRequest.Passward==user.Password)
                {
                    netMessage.Response.userLogin.Result = Result.Success;
                    netMessage.Response.userLogin.Errormsg = "None";
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

    }
}
