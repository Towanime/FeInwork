using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;

namespace FeInwork.Core.Base
{
    public abstract class BaseComponent: IComponent
    {
        // entidad dueña de este componente
        protected IEntity owner;

        public BaseComponent(IEntity owner)
        {
            this.owner = owner;
        }

        // se pone abstract para obligar a los componentes a implementarlo
        public abstract void initialize();

        public IEntity Owner
        {
            get { return owner; }
        }
    }
}
