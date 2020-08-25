using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Network.NetClient.Instance.Init("127.0.0.1", 8000);
        Network.NetClient.Instance.Connect();

        SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();//新建一个消息
        msg.Request = new SkillBridge.Message.NetMessageRequest();//新建一个请求
        SkillBridge.Message.FirstTestRequest firstTestRequest = new SkillBridge.Message.FirstTestRequest();//新建子请求
        firstTestRequest.Helloworld = "Hello World";//消息值

        msg.Request.firstRequest = firstTestRequest;//绑定
        /*
        SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
        msg.Request = new SkillBridge.Message.NetMessageRequest();
        msg.Request.firstRequest = new SkillBridge.Message.FirstTestRequest();
        msg.Request.firstRequest.Helloworld = "Hello World";
        */
        Network.NetClient.Instance.SendMessage(msg);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
