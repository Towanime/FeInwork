using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using AlchemistDemo.alchemist.events;

namespace AlchemistDemo.alchemist.listeners
{
    public interface PositionChangeListener : IEventListener
    {
        void invoke(PositionEvent eventObject);
    }
}
