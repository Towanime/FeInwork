﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.events;

namespace FeInwork.FeInwork.listeners
{
    interface EntityAddedListener : IEventListener
    {
        void invoke(EntityAddedEvent eventArgs);
    }
}