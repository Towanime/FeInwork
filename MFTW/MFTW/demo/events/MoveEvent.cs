using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.FeInwork.events
{
    public class MoveEvent : AbstractEvent
    {
        public enum MOVE_TYPE
        {
            WALK,
            JUMP,
            ACTION,
            FLY
        }

        private MOVE_TYPE moveType;

        private MoveEvent(object origin, MOVE_TYPE moveType)
            : base(origin, EventType.MOVE_EVENT)
        {
            this.moveType = moveType;
        }

        public static MoveEvent Create(object origin, MOVE_TYPE moveType)
        {
            MoveEvent returningEvent = EventManager.Instance.GetEventFromType<MoveEvent>(EventType.MOVE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new MoveEvent(origin, moveType));
            }
            else
            {
                returningEvent.moveType = moveType;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public MOVE_TYPE MoveType
        {
            get
            {
                return this.moveType;
            }
        }
    }
}
