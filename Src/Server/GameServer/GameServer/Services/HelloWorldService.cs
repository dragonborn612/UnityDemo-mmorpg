using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class HelloWorldService:Singleton<HelloWorldService>
    {
        public void Init()
        {
            
        }
        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.OnfirstTestRequst);

                
        }
        void OnfirstTestRequst(NetConnection<NetSession> sender,FirstTestRequest request)
        {
            Log.InfoFormat("UserLoginRequest: Helloworld:{0}", request.Helloworld); 
        }

    }
}
