using System;

namespace EmberToolkit.Common.Attributes
{
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class SaveFieldAttribute : Attribute
        {
            // You can optionally include additional properties or parameters in the attribute
        }
}
