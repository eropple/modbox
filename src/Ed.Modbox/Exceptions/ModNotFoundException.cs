using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ed.Modbox.Exceptions
{
    public class ModNotFoundException : Exception
    {
        public ModNotFoundException(ModDefinition definition)
            : base(String.Format("Mod not found: {0}", definition.ToString((true))))
        {
        }
    }
}
