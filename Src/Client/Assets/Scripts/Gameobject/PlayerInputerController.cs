using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Gameobject;

public class PlayerInputerController : MonoBehaviour {

    public Rigidbody rb;

    CharacterState state;

    public Character character;

    public float rotalSpeed=2.0f;

    public float turnAgle = 10;

    public int speed;
    public EntiyController entiyController;
    public bool isPlayer;

	// Use this for initialization
	void Start () {
        state = CharacterState.Idle;
        if (this.character == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo nCharacterInfo = new NCharacterInfo();
            nCharacterInfo.Id = 1;
            nCharacterInfo.Tid = 1;
            nCharacterInfo.Entity = new NEntity();
            nCharacterInfo.Entity.Position = new NVector3();
            nCharacterInfo.Entity.Direction = new NVector3();
            nCharacterInfo.Entity.Direction.X = 0;
            nCharacterInfo.Entity.Direction.Y = 100;
            nCharacterInfo.Entity.Direction.Z = 0;
            character = new Character(nCharacterInfo);
            if (entiyController!=null)
            {
                entiyController.entity = this.character;
            }


        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (character==null)
        {
            return;
        }
        if (!isPlayer)
        {
            return;
        }
        float v = Input.GetAxis("Vertical");
        if (v > 0.01f)
        {
            if (state!=CharacterState.Move)
            {
                state = CharacterState.Move;
                
                SendEntityEvent(EntityEvent.MoveFwd);//动画状态改变
                this.character.MoveForward();//改变速度

            }
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f)/100f;//9.81为了移动更平滑
            //Debug.Log(rb.velocity);
            //Debug.Log(character.speed);
        }
        else if (v<-0.01f)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
               
                SendEntityEvent(EntityEvent.MoveBack);//动画状态改变

            }
            this.character.MoveBack();
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f)/100f;
        }
        else
        {
            if (state!=CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                character.Stop();
                SendEntityEvent(EntityEvent.Idle);
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h<-0.1||h>0.1)
        {
            this.transform.Rotate(0, h * rotalSpeed, 0);//绕y轴旋转
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rotate = new Quaternion();
            rotate.SetFromToRotation(dir, this.transform.forward);//从第一个参数旋转到第二个参数
            if (rotate.eulerAngles.y > this.turnAgle && rotate.eulerAngles.y < (360 - turnAgle))//10度以内忽略不计
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                
                this.SendEntityEvent(EntityEvent.None);
            }
            //rb.transform.forward = this.transform.forward;
        }
            
    }
    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        if (character == null)
        {
            return;
        }
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);//magnitude向量的模
        this.lastPos = this.rb.transform.position;
        if ((GameObjectTool.WorldToLogic(rb.transform.position)-this.character.position).magnitude>50)//物理位置和逻辑位置大于50
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;
        //var dir = GameObjectTool.LogicToWorld(character.direction);
        //var rot = new Quaternion();
        //rot.SetFromToRotation(dir, this.transform.forward);
        //if (rot.eulerAngles.y>this.turnAgle&&rot.eulerAngles.y<(360-this.turnAgle))
        //{
        //    character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
        //}
    }

    void SendEntityEvent(EntityEvent entityEvent)
    {
        if (entiyController!=null)
        {
            entiyController.OnEntityEnvent(entityEvent);
        }
    }
}
