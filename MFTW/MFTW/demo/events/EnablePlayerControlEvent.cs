using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using AlchemistDemo.core.interfaces;
using AlchemistDemo.core.items;

namespace AlchemistDemo.alchemist.events
{
    public class EnablePlayerControlEvent : AbstractEvent
    {
        public EnablePlayerControlEvent(object origin) :
            base(origin, EventType.ENABLE_PLAYER_CONTROL_EVENT)
        {

        }
    }
}
