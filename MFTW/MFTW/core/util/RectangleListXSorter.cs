using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.core.util
{
    public class RectangleListXSorter : IComparer<Rectangle>
    {
        public int Compare(Rectangle obj1, Rectangle obj2)
        {
            return obj1.X.CompareTo(obj2.X);
        }
    }
}
