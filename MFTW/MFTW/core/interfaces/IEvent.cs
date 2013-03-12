using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Util;

namespace FeInwork.Core.Interfaces
{
    public interface IEvent
    {
        int Type
        {
            get;
        }

        bool IsAvailable
        {
            get;
            set;
        }

        object Origin
        {
            get;
        }
    }
}
