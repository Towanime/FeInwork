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
    public class PositionChangeRequestEvent : AbstractEvent
    {
        private Vector2 currentPosition;
        private Vector2 projectedDistance;

        private PositionChangeRequestEvent(object origin, Vector2 currentPosition, Vector2 projectedDistance)
            : base(origin, EventType.POSITION_CHANGE_REQUEST_EVENT)
        {
            this.currentPosition = currentPosition;
            this.projectedDistance = projectedDistance;
        }

        public static PositionChangeRequestEvent Create(object origin, Vector2 currentPosition, Vector2 projectedDistance)
        {
            PositionChangeRequestEvent returningEvent = EventManager.Instance.GetEventFromType<PositionChangeRequestEvent>(EventType.POSITION_CHANGE_REQUEST_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PositionChangeRequestEvent(origin, currentPosition, projectedDistance));
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
