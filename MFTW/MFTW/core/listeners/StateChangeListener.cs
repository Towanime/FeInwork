using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Events;
using FeInwork.Core.Base;
using FeInwork.core.events;

namespace FeInwork.Core.Listeners
{
    public interface StateChangeListener: IEventListener
    {
        void invoke(StateChangeEvent eventObject);
    }
}
