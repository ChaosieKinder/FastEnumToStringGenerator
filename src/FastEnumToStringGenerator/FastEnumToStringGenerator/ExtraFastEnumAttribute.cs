using System;
using System.Collections.Generic;
using System.Text;

namespace FastEnumToStringGenerator
{
    /// <summary>
    /// Adds an extra enum to the FastEnum code generator. Great for system enums and enums from other librarys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExtraFastEnumAttribute : Attribute 
    {
        public Type Type { get; }

        public ExtraFastEnumAttribute(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException("Specified type must be an Enum");

            Type = t;
        }
    }
}
