using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeInwork.Core.Util
{
   // public class EventType
   // {
    public class EventType
    {
        //
        public const int ACTION_CHANGE_EVENT = 0;
        //
        public const int DAMAGE_EVENT = 1;
        public const int INPUT_EVENT = 2;
        public const int PROPERTY_CHANGE_EVENT = 3;
        public const int STATE_CHANGE_EVENT = 4;
        public const int PICKUP_EVENT = 5;
        public const int ENTITY_ADDED_EVENT = 6;
        // aparte del demo
        public const int TRANSITION_OVER_EVENT = 7;
        public const int TRANCE_BEGIN_EVENT = 8;
        public const int TRANCE_END_EVENT = 9;
        public const int DEAD_EVENT = 10;
        public const int DEAD_ACTION_EVENT = 11;
        public const int CURE_EVENT = 12;
        public const int MOVE_EVENT = 13;
        public const int POSITION_CHANGE_REQUEST_EVENT = 14;
        public const int COLLISION_EVENT = 15;
        public const int FORCE_APPLIED_EVENT = 16;
        public const int ENABLE_STATE_CHANGE_EVENT = 17;
        public const int ALERT_ACTION_EVENT = 18;
        public const int PERFORM_SPECIAL_ACTION_EVENT = 19;
        public const int ENABLE_PHYSICS_EVENT = 20;
        public const int ENTITY_ON_SURFACE_EVENT = 21;
        public const int ENABLE_VANQUISH_MODE_EVENT = 22;
        // al entrar en rango de un trigger
        public const int TRIGGER_IN_RANGE_EVENT = 23;
        // al salir de rando de un trigger
        public const int TRIGGER_OUT_RANGE_EVENT = 24;
        public const int PHYSICAL_ATTACK_EVENT = 25;
        // evento de controles activos o no
        public const int CONTROL_ENABLE_STATE_CHANGE_EVENT = 26;
        public const int ENABLE_INVENTORY_EVENT = 27;
        public const int HARMED_EVENT = 28;
        public const int POSITION_CHANGED_EVENT = 29;
        public const int FOCUS_BEGIN_EVENT = 30;
        public const int FOCUS_END_EVENT = 31;
        public const int ELEMENT_QUEUE_EVENT = 32;
        public const int ACTIVATE_ALCHEMY_EVENT = 33;
        public const int DEACTIVATE_ALCHEMY_EVENT = 34;
        public const int DISABLE_INVENTORY_EVENT = 35;
        public const int DRINKING_EVENT = 36;
        public const int START_WATER_ALCHEMY_CONTROL_EVENT = 37;
        public const int END_WATER_ALCHEMY_CONTROL_EVENT = 38;
        public const int ELEMENTAL_DAMANGE_EVENT = 39;
        public const int SURFACE_CONTACT_EVENT = 40;
    };
    //}
}
