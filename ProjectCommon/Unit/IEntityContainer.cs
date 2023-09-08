using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCommon.Unit
{
    public interface IEntityContainer
    {
        T GetObject<T>();
        T GetObject<T, Key>(Key key);
        T GetObject<T, Key1, Key2>(Key1 key1, Key2 key2);

        bool AddObject<T>(T obj);
        bool AddObject<T, Key>(T obj);
        bool AddObject<T, Key1, Key2>(T obj);

        bool RemoveObject<T>(T obj);
        bool RemoveObject<T, Key>(Key key);
        bool RemoveObject<T, Key1, Key2>(Key1 key1, Key2 key2);
    }

    public class DefaultEntityContainer : IEntityContainer
    {
        public bool AddObject<T>(T obj)
        {
            IEntityContainer<T> container = this as IEntityContainer<T>;
            if (container == null)
            {
                return false;
            }

            return container.AddObject(obj);
        }

        public bool AddObject<T, Key>(T obj)
        {
            IEntityContainer<T, Key> container = this as IEntityContainer<T, Key>;
            if (container == null)
            {
                return false;
            }

            return container.AddObject(obj);
        }

        public bool AddObject<T, Key1, Key2>(T obj)
        {
            IEntityContainer<T, Key1, Key2> container = this as IEntityContainer<T, Key1, Key2>;
            if (container == null)
            {
                return false;
            }

            return container.AddObject(obj);
        }

        public T GetObject<T>()
        {
            IEntityContainer<T> container = this as IEntityContainer<T>;
            if (container == null)
            {
                return default(T);
            }

            return container.GetObject();
        }

        public T GetObject<T, Key>(Key key)
        {
            IEntityContainer<T, Key> container = this as IEntityContainer<T, Key>;
            if (container == null)
            {
                return default(T);
            }

            return container.GetObject(key);
        }

        public T GetObject<T, Key1, Key2>(Key1 key1, Key2 key2)
        {
            IEntityContainer<T, Key1, Key2> container = this as IEntityContainer<T, Key1, Key2>;
            if (container == null)
            {
                return default(T);
            }

            return container.GetObject(key1, key2);
        }

        public bool RemoveObject<T>(T obj)
        {
            IEntityContainer<T> container = this as IEntityContainer<T>;
            if (container == null)
            {
                return false;
            }

            return container.RemoveObject(obj);
        }

        public bool RemoveObject<T, Key>(Key key)
        {
            IEntityContainer<T, Key> container = this as IEntityContainer<T, Key>;
            if (container == null)
            {
                return false;
            }

            return container.RemoveObject(key);
        }

        public bool RemoveObject<T, Key1, Key2>(Key1 key1, Key2 key2)
        {
            IEntityContainer<T, Key1, Key2> container = this as IEntityContainer<T, Key1, Key2>;
            if (container == null)
            {
                return false;
            }

            return container.RemoveObject(key1, key2);
        }
    }

    public interface IEntityContainer<T>
    {
        bool AddObject(T obj);
        T GetObject();
        bool RemoveObject(T obj);
    }

    public interface IEntityContainer<T, Key>
    {
        bool AddObject(T obj);
        T GetObject(Key key);
        T GetObject(Func<T, bool> patten);
        bool RemoveObject(Key key);
    }

    public interface IEntityContainer<T, Key1, Key2>
    {
        bool AddObject(T obj);
        T GetObject(Key1 key1, Key2 key2);
        T GetObject(Func<T, bool> patten);
        bool RemoveObject(Key1 key1, Key2 key2);
    }
}
