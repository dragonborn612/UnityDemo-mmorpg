using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }


        public void Disconnected()
        {
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }


        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (this.Character != null && this.Character.StatusManager.HasStatus)
                {
                    this.Character.StatusManager.ApplyResponse(Response);
                }
                byte[] data = PackageHandler.PackMessage(response);//打包
                response = null;
                return data;
            }
            return null;
        }
    }
}
