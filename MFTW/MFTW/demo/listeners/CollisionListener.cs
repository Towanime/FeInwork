using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.events;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.listeners
{
    public interface CollisionListener : IEventListener
    {
        void invoke(CollisionEvent eventObject);

        Color? ColorTag { get; }
    }
}
