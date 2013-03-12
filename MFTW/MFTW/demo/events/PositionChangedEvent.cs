using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.FeInwork.events
{
    public class PositionChangedEvent : AbstractEvent
    {
        private Vector2 currentPosition;
        private Vector2 projectedDistance;

        private PositionChangedEvent(object origin, ref Vector2 currentPosition, ref Vector2 projectedDistance)
            : base(origin, EventType.POSITION_CHANGED_EVENT)
        {
            this.currentPosition = currentPosition;
            this.projectedDistance = projectedDistance;
        }

        public static PositionChangedEvent Create(object origin, ref Vector2 currentPosition, ref Vector2 projectedDistance)
        {
            PositionChangedEvent returningEvent = EventManager.Instance.GetEventFromType<PositionChangedEvent>(EventType.POSITION_CHANGED_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PositionChangedEvent(origin, ref currentPosition, ref projectedDistance));
            }
            else
            {
                returningEvent.projectedDistance = projectedDistance;
                returningEvent.currentPosition = currentPosition;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public Vector2 CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
        }

        public Vector2 ProjectedDistance
        {
            get
            {
                return this.projectedDistance;
            }
        }
    }
}
