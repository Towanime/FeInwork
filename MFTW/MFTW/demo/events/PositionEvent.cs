using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace AlchemistDemo.alchemist.events
{
    public class PositionEvent : AbstractEvent
    {
        private Vector2 currentPosition;
        private Vector2 projectedDistance;
        private bool corrected;

        private PositionEvent(object origin, Vector2 currentPosition, Vector2 projectedDistance, bool corrected)
            : base(origin, EventType.POSITION_EVENT)
        {
            this.currentPosition = currentPosition;
            this.projectedDistance = projectedDistance;
            this.corrected = corrected;
        }

        public static PositionEvent Create(object origin, Vector2 currentPosition, Vector2 projectedDistance, bool corrected)
        {
            PositionEvent returningEvent = EventManager.Instance.GetEventFromType<PositionEvent>(EventType.POSITION_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PositionEvent(origin, currentPosition, projectedDistance, corrected));
            }
            else
            {
                returningEvent.corrected = corrected;
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

        public bool Corrected
        {
            get
            {
                return this.corrected;
            }
        }
    }
}
