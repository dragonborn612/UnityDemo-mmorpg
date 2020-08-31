using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
namespace Entities
{
    public class Entity
    {
        public int entityId;

        public Vector3Int position;//位置
        public Vector3Int direction;//方向
        public int speed;//速度

        private NEntity entityData;

        public NEntity EntityData
        {
            get
            {
                UpdateEntityData();
                return entityData;
            }

            set
            {
                entityData = value;
                SetEntityData(value);

            }
        }
        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="nEntity"></param>
        public Entity( NEntity nEntity)
        {
            entityId = nEntity.Id;
            this.EntityData = nEntity;
        }
        /// <summary>
        /// 给字段赋值 Nvector3转vector3Int
        /// </summary>
        /// <param name="nEntity"></param>
        public void SetEntityData(NEntity nEntity)
        {
            this.position = this.position.FromNVector3(nEntity.Position);//将Nvector3的position转化为vector3Int类型的并赋值
            this.direction = this.direction.FromNVector3(nEntity.Direction);
            this.speed = nEntity.Speed;

        }
        
        public virtual void OnUpdate(float delta)
        {
            if (this.speed!=0)
            {
                //Debug.LogFormat("{0}:sped:{1}",this.entityId, this.speed);
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
               
            }
        }
        /// <summary>
        /// 实体属性向消息属性更新
        /// </summary>
        public void UpdateEntityData()
        {

            entityData.Speed = this.speed;
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
        }
    }
}
