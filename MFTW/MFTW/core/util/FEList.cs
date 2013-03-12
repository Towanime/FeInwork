using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.events;

namespace FeInwork.core.util
{
    public class FEList<T> : List<T>
    {
        public FEList()
            : base()
        {
        }
        
       public void AddEntity(T item, Boolean invoke, object origin) 
       {   
           base.Add(item);
           if (invoke)
           {
               EventManager.Instance.fireEvent(EntityAddedEvent.Create(origin, (IEntity)item));
           }
       }

       public void AddEntity(T item)
       {
           AddEntity(item, false, null);
       }
    }
}
