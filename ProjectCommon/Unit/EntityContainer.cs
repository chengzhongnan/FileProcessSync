using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCommon.Unit
{
    public class EntityContainer<TParentContainer, TEntityType> : DefaultEntityContainer, IEntityBase<TParentContainer>
        where TParentContainer : IEntityContainer
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        public TEntityType Type { get; private set; }
        public EntityContainer(TParentContainer parent, TEntityType type)
        {
            Type = type;
            ParentMgr = parent;
            //Init();
        }

        /// <summary>
        /// 父管理
        /// </summary>
        protected TParentContainer ParentMgr { get; private set; }


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

        public TParentContainer GetParent()
        {
            return ParentMgr;
        }
    }
}
