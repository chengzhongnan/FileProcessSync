using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCommon.Unit
{

    public class EntityMgr : DefaultEntityContainer, IEntityContainer<EntityBase, short>
    {
        protected Dictionary<short, EntityBase> _entityMap { get; private set; }

        public EntityMgr()
        {
            _entityMap = new Dictionary<short, EntityBase>();
        }

        /// <summary>
        /// 注册实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool CreateEntity<TEntity>() where TEntity : EntityBase, new()
        {
            TEntity entity = new TEntity();
            return AddObject(entity);
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T FindEntity<T>(short type) where T: EntityBase
        {
            return FindEntity(type) as T;
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityBase FindEntity(short type)
        {
            if (_entityMap.ContainsKey(type))
                return _entityMap[type];

            return null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            foreach (var it in _entityMap)
            {
                it.Value.Init();
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {
            foreach (var it in _entityMap)
            {
                it.Value.Release();
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Heartbeat(double elapsed)
        {
            foreach (var it in _entityMap)
            {
                it.Value.Heartbeat(elapsed);
            }
        }

        bool IEntityContainer<EntityBase, short>.AddObject(EntityBase entity)
        {
            if (!_entityMap.ContainsKey(entity.Type))
            {
                _entityMap.Add(entity.Type, entity);
                return true;
            }
            return false;
        }

        EntityBase IEntityContainer<EntityBase, short>.GetObject(short key)
        {
            return FindEntity(key);
        }

        EntityBase IEntityContainer<EntityBase, short>.GetObject(Func<EntityBase, bool> patten)
        {
            foreach(var kv in _entityMap)
            {
                if (patten(kv.Value))
                {
                    return kv.Value;
                }
            }
            return null;
        }

        bool IEntityContainer<EntityBase, short>.RemoveObject(short key)
        {
            if (_entityMap.ContainsKey(key))
            {
                _entityMap.Remove(key);
                return true;
            }

            return false;
        }
    }

    public class EntityMgr<T, Key> : DefaultEntityContainer, IEntityContainer<T, Key>
        where T : EntityBase<Key>
    {
        protected Dictionary<Key, T> _entityMap { get; private set; }

        public EntityMgr()
        {
            _entityMap = new Dictionary<Key, T>();
        }

        /// <summary>
        /// 注册实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool CreateEntity<TEntity>() where TEntity : EntityBase, new()
        {
            TEntity entity = new TEntity();
            return AddObject(entity);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            foreach (var it in _entityMap)
            {
                it.Value.Init();
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {
            foreach (var it in _entityMap)
            {
                it.Value.Release();
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="elapsed"></param>
        public virtual void Heartbeat(double elapsed)
        {
            foreach (var it in _entityMap)
            {
                it.Value.Heartbeat(elapsed);
            }
        }

        bool IEntityContainer<T, Key>.AddObject(T obj)
        {
            if (_entityMap.ContainsKey(obj.Type))
            {
                return false;
            }

            _entityMap.Add(obj.Type, obj);

            return true;
        }

        T IEntityContainer<T, Key>.GetObject(Key key)
        {
            if (_entityMap.ContainsKey(key))
            {
                return _entityMap[key];
            }

            return default(T);
        }

        T IEntityContainer<T, Key>.GetObject(Func<T, bool> patten)
        {
            foreach(var kv in _entityMap)
            {
                if (patten(kv.Value))
                {
                    return kv.Value;
                }
            }

            return default(T);
        }

        bool IEntityContainer<T, Key>.RemoveObject(Key key)
        {
            return _entityMap.Remove(key);
        }
    }
}
