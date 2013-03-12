using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.FeInwork.events
{
    public class ForceAppliedEvent : AbstractEvent
    {
        private double angle;
        private float magnitude;

        private ForceAppliedEvent(object origin, double angle, float magnitude)
            : base(origin, EventType.FORCE_APPLIED_EVENT)
        {
            this.angle = angle;
            this.magnitude = magnitude;
        }

        public static ForceAppliedEvent Create(object origin, double angle, float magnitude)
        {
            ForceAppliedEvent returningEvent = EventManager.Instance.GetEventFromType<ForceAppliedEvent>(EventType.FORCE_APPLIED_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new ForceAppliedEvent(origin, angle, magnitude));
            }
            else
            {
                returningEvent.angle = angle;
                returningEvent.magnitude = magnitude;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public double Angle
        {
            get { return this.angle; }
        }

        public float Magnitude
        {
            get { return this.magnitude; }
        }
    }
}
