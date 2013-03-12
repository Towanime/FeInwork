using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeInwork.core.util
{
    public class TipoAttribute : Attribute
    {
        public string tipo;
        public bool isInXml;

        public TipoAttribute(String type)
        {
            tipo = type;
            isInXml = true;
        }

        public TipoAttribute(String type, bool isInXml)
        {
            tipo = type;
            this.isInXml = isInXml;
        }

        public override string ToString()
        {
            return tipo;
        }
    }
}
