using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.core.interfaces;

namespace FeInwork.Core.Interfaces
{
    // estas interfaces que hereda son la base de una entidad
    public interface IEntity : IEntityComponentCollection, IPropertyContainer, IStateContainer
    {
        string Id
        {
            get;
        }
    }
}
