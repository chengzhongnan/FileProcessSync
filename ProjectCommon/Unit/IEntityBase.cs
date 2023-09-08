using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCommon.Unit
{
    //public interface IEntityBase
    //{
    //}

    public interface IEntityBase<T>
        where T : IEntityContainer
    {
        T GetParent();
    }
}
