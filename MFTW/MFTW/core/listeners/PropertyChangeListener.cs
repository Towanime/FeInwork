using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Events;
using FeInwork.Core.Base;

namespace FeInwork.Core.Listeners
{
    public interface PropertyChangeListener: IEventListener
    {
        void invoke(PropertyChangeEvent eventObject);
    }
}
