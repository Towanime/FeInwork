using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlchemistDemo.alchemist.events;
using FeInwork.Core.Interfaces;

namespace AlchemistDemo.alchemist.listeners
{
    public interface GrabListener : IEventListener
    {
        void invoke(GrabEvent eventObject);
    }
}
