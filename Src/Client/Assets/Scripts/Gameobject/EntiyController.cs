using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Gameobject;
using SkillBridge.Message;
using Assets.Scripts.Managers;
using Assets.Scripts.Gameobject;
using System;

public class EntiyController : MonoBehaviour, IEntityNotiy
{

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaceState;

    public Entity entity;

    public Vector3 position;
    public Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;
    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;
    public RideController rideController;
    private int currentRideid = 0;
    public Transform rideBone;
    // Use this for initialization
    void Start() {
        if (entity != null)
        {
            EntityManager.Instance.RegiestEntityChangeNotify(entity.entityId, this);
            this.UpdataTransforn();

        }
        if (!isPlayer)
        {
            this.rb.useGravity = false;
        }
    }
    private void OnDestroy()
    {
        if (entity != null)
        {
            Debug.LogFormat("{0} Ondestroy:ID:{1} Pos:{2} Dir:{3} Spd:{4}", this.name, entity.entityId, entity.position, entity.direction, entity.speed);
            if (UIWorldElementManager.Instance != null)
            {
                UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        if (this.entity == null)
        {
            return;
        }
        this.entity.OnUpdate(Time.fixedDeltaTime);
        if (!isPlayer)
        {
            this.UpdataTransforn();
        }
    }
    void UpdataTransforn()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);//将刚体移动到参数位置
        this.transform.forward = this.direction;

        this.lastPosition = this.position;
        this.lastRotation = rotation;
    }
    /// <summary>
    /// 动画状态机条件改变
    /// </summary>
    /// <param name="entityEvent"></param> //接口实现 同步状态
    public void OnEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                {
                    this.Ride(param);
                }
                break;
        }
        if (this.rideController != null)
        {
            this.rideController.onEntityEvent(entityEvent, param);
        }
    }



    //接口实现
    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
        Destroy(this.gameObject);
    }
    //接口实现
    public void OnEntityChange(Entity entity)
    {
        Debug.LogFormat("OnEntityChange:id:{0} pos:{1}:Dir:{2} Spd{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }
    public void Ride(int rideId)
    {
        if (currentRideid==rideId)
        {
            return;
        }
        currentRideid = rideId;
        if (rideId>0)
        {
            this.rideController = GammaObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;

        }
        if (this.rideController==null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);
        }
        else
        {
            this.rideController.SetRider(this);
            this.anim.SetLayerWeight(1, 1);
        }
    }
    public void SetRidePotision(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

}
