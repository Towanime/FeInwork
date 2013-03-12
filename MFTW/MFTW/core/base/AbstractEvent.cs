using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Util;
using FeInwork.Core.Interfaces;

namespace FeInwork.Core.Base
{
    // base pata todos los tipos de eventos!
    public abstract class AbstractEvent: IEvent
    {
        //Tipo de evento que se identifica,
        private int eventType;
        protected object origin;
        private bool isAvailable;

        public AbstractEvent(object origin, int eventType)
        {
            this.origin = origin;
            this.eventType = eventType;
            this.isAvailable = true;
        }

        public int Type
        {
            get { return eventType; }
        }

        public object Origin 
        {
            get { return origin; }
        }

        public bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }
    }
}
