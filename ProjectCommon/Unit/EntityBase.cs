using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCommon.Unit
{
    public abstract class EntityBase : IEntityBase<EntityMgr>
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public short Type { get; private set; }
        public EntityBase(EntityMgr mgr, short type)
        {
            Type = type;
            ParentMgr = mgr;
            //Init();
        }

        /// <summary>
        /// 父管理
        /// </summary>
        protected EntityMgr ParentMgr { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {

        }

        /// <summary>
        /// 心跳逻辑
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Heartbeat(double elapsed)
        {

        }

        public EntityMgr GetParent()
        {
            return ParentMgr;
        }
    }

    public abstract class EntityBase<TypeKey> : IEntityBase<EntityMgr>
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public TypeKey Type { get; private set; }
        public EntityBase(EntityMgr mgr, TypeKey type)
        {
            Type = type;
            ParentMgr = mgr;
            //Init();
        }

        /// <summary>
        /// 父管理
        /// </summary>
        protected EntityMgr ParentMgr { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {

        }

        /// <summary>
        /// 心跳逻辑
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Heartbeat(double elapsed)
        {

        }

        public EntityMgr GetParent()
        {
            return ParentMgr;
        }
    }
}
