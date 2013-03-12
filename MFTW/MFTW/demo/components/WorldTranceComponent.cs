using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using AlchemistDemo.alchemist.listeners;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using AlchemistDemo.alchemist.entities;
using AlchemistDemo.alchemist.events;
using AlchemistDemo.alchemist.world;

namespace AlchemistDemo.alchemist.components
{
    public class WorldTranceComponent : BaseComponent, TranceModeListener
    {
        public WorldTranceComponent(RoomEntity owner)
            : base(owner)
        {
            initialize();
        }

        public override void initialize()
        {
            // registrase
            EventManager.Instance.addListener(EventType.TRANCE_BEGIN_EVENT, this);
            EventManager.Instance.addListener(EventType.TRANCE_END_EVENT, this);
        }

        public void invoke(TranceBeginEvent eventArgs)
        {
            ((RoomEntity)owner).IsTranceModeOn = true;
        }

        public void invoke(TranceEndEvent eventArgs)
        {
            ((RoomEntity)owner).IsTranceModeOn = false;
        }
    }
}
